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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Reservation -> ApplicationUser (FK UserId), без каскадно бришење
            builder.Entity<Reservation>(entity =>
            {
                entity.HasOne(r => r.User)
                      .WithMany() // ако во ApplicationUser немаш колекција Reservations
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            //  индекс за побрзи пребарувања
            builder.Entity<Reservation>()
                   .HasIndex(r => new { r.ScreeningId, r.UserId });
        }
    }
}
