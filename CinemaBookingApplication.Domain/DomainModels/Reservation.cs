using CinemaBookingApplication.Domain.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaBookingApplication.Domain.DomainModels;
public class Reservation : BaseEntity
{
    public Guid ScreeningId { get; set; }
    public Screening? Screening { get; set; }

    public string UserId { get; set; } = default!;       
    public ApplicationUser? User { get; set; }           
    public int Quantity { get; set; }
   
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
}
