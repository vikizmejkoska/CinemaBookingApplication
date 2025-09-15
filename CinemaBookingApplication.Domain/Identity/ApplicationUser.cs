using Microsoft.AspNetCore.Identity;

namespace CinemaBookingApplication.Domain.Identity;
public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}
