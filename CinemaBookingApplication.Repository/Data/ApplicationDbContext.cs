using Microsoft.EntityFrameworkCore;
using CinemaBookingApplication.Domain.DomainModels;
using CinemaBookingApplication.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CinemaBookingApplication.Repository.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Hall> Halls => Set<Hall>();
        public DbSet<Screening> Screenings => Set<Screening>();
        public DbSet<Reservation> Reservations => Set<Reservation>();

  
    }
}
