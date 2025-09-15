namespace CinemaBookingApplication.Service.Options;

public class TmdbOptions
{
    public string ApiKey { get; set; } = string.Empty;   // v4 Bearer token
    public string BaseUrl { get; set; } = "https://api.themoviedb.org/3";
    public string ImageBase { get; set; } = "https://image.tmdb.org/t/p/w500";
}
