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
        // IMPORTANT: relative paths (no leading slash)
        string path = query.Equals("popular", StringComparison.OrdinalIgnoreCase)
            ? "movie/popular"
            : query.Equals("top_rated", StringComparison.OrdinalIgnoreCase)
                ? "movie/top_rated"
                : $"search/movie?query={Uri.EscapeDataString(query)}";

        _log.LogInformation("TMDB GET {Url}", new Uri(_http.BaseAddress!, path));

        TmdbList? res;
        try
        {
            res = await _http.GetFromJsonAsync<TmdbList>(path);
        }
        catch (HttpRequestException ex)
        {
            _log.LogError(ex, "TMDB import failed for path {Path}", path);
            throw new Exception("Import failed: could not reach TMDB (check ApiKey / network).");
        }

        var results = res?.results ?? new List<TmdbMovie>();

        var mapped = results.Select(r => new Movie
        {
            Id = Guid.NewGuid(),
            Title = string.IsNullOrWhiteSpace(r.title) ? "Untitled" : r.title.Trim(),
            Overview = r.overview ?? "",
            Runtime = 0,
            Rating = r.vote_average ?? 0,
            PosterUrl = string.IsNullOrWhiteSpace(r.poster_path)
                ? null
                : _opt.ImageBase.TrimEnd('/') + r.poster_path
        })
        .GroupBy(m => m.Title.ToLowerInvariant())
        .Select(g => g.First())
        .ToList();

        if (mapped.Count == 0) return 0;

        _movies.InsertMany(mapped);
        return mapped.Count;
    }

    private sealed class TmdbList { public List<TmdbMovie> results { get; set; } = new(); }
    private sealed class TmdbMovie
    {
        public string? title { get; set; }
        public string? overview { get; set; }
        public double? vote_average { get; set; }
        public string? poster_path { get; set; }
    }
}
