// CinemaBookingApplication.Service/Implementation/TmdbFetchService.cs
using System.Net.Http.Json;
using CinemaBookingApplication.Domain.DomainModels;
using CinemaBookingApplication.Service.Interface;
using CinemaBookingApplication.Service.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CinemaBookingApplication.Service.Implementation;

public class TmdbFetchService : IDataFetchService
{
    private readonly HttpClient _http;
    private readonly IMovieService _movies;
    private readonly ILogger<TmdbFetchService> _log;
    private readonly TmdbOptions _opt;

    public TmdbFetchService(
        HttpClient http,
        IMovieService movies,
        ILogger<TmdbFetchService> log,
        IOptions<TmdbOptions> opt)
    {
        _http = http;
        _movies = movies;
        _log = log;
        _opt = opt.Value;
    }

    public async Task<int> ImportMoviesFromTmdbAsync(string query = "popular")
    {
        // 1) земи листа
        string path = query.Equals("popular", StringComparison.OrdinalIgnoreCase)
            ? "movie/popular"
            : query.Equals("top_rated", StringComparison.OrdinalIgnoreCase)
                ? "movie/top_rated"
                : $"search/movie?query={Uri.EscapeDataString(query)}";

        _log.LogInformation("TMDB LIST {Url}", new Uri(_http.BaseAddress!, path));

        TmdbList? list;
        try
        {
            list = await _http.GetFromJsonAsync<TmdbList>(path);
        }
        catch (HttpRequestException ex)
        {
            _log.LogError(ex, "TMDB import failed for path {Path}", path);
            throw new Exception("Import failed: could not reach TMDB (check ApiKey / network).");
        }

        var results = list?.results ?? new List<TmdbMovie>();
        if (results.Count == 0) return 0;

        // 2) земи постоечки наслови за да избегнеме дупликати
        var existingTitles = new HashSet<string>(
            _movies.All().Select(m => m.Title.ToLowerInvariant()));

        // 3) за секоја ставка – земи детали за runtime
        var mapped = new List<Movie>();
        foreach (var r in results)
        {
            if (string.IsNullOrWhiteSpace(r.title)) continue;
            var title = r.title.Trim();
            var lower = title.ToLowerInvariant();
            if (existingTitles.Contains(lower)) continue; // прескокни ако веќе постои

            int runtime = 0;
            try
            {
                if (r.id > 0)
                {
                    var detail = await _http.GetFromJsonAsync<TmdbMovieDetail>($"movie/{r.id}");
                    runtime = detail?.runtime ?? 0;
                }
            }
            catch (HttpRequestException ex)
            {
                _log.LogWarning(ex, "Failed to fetch details for movie id {Id}", r.id);
            }

            var mv = new Movie
            {
                Id = Guid.NewGuid(),
                Title = title,
                Overview = r.overview ?? "",
                Runtime = runtime,                                  // <-- сега ќе е пополнето
                Rating = r.vote_average ?? 0,
                PosterUrl = string.IsNullOrWhiteSpace(r.poster_path)
                    ? null
                    : _opt.ImageBase.TrimEnd('/') + r.poster_path
            };

            mapped.Add(mv);
            existingTitles.Add(lower);
        }

        if (mapped.Count == 0) return 0;

        _movies.InsertMany(mapped);
        return mapped.Count;
    }

    // DTO-и што ги користиме од TMDB
    private sealed class TmdbList { public List<TmdbMovie> results { get; set; } = new(); }

    private sealed class TmdbMovie
    {
        public int id { get; set; }                   // потребно за detail повик
        public string? title { get; set; }
        public string? overview { get; set; }
        public double? vote_average { get; set; }
        public string? poster_path { get; set; }
    }

    private sealed class TmdbMovieDetail
    {
        public int? runtime { get; set; }             // во минути
    }
}
