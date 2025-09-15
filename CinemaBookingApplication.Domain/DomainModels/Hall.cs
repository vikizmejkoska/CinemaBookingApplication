namespace CinemaBookingApplication.Domain.DomainModels;
public class Hall : BaseEntity
{
    public string Name { get; set; } = default!;
    public int Capacity { get; set; }
    public virtual ICollection<Screening>? Screenings { get; set; }
}
