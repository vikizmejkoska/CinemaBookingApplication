namespace CinemaBookingApplication.Domain.DomainModels;
public class Movie : BaseEntity
{
    public string Title { get; set; } = default!;
    public string? Overview { get; set; }
    public int Runtime { get; set; } // минути
    public double Rating { get; set; } // 0-10
    public string? PosterUrl { get; set; }
    public virtual ICollection<Screening>? Screenings { get; set; }
}
