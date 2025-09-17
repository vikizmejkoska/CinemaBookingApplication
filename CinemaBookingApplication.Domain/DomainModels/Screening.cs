using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaBookingApplication.Domain.DomainModels;
public class Screening : BaseEntity
{
    public Guid MovieId { get; set; }
    public Movie? Movie { get; set; }

    public Guid HallId { get; set; }
    public Hall? Hall { get; set; }

    public DateTime StartTime { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public virtual ICollection<Reservation>? Reservations { get; set; }
}
