using Microsoft.EntityFrameworkCore;
using TicketBooking.Core.Entities;

namespace TicketBooking.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Screening> Screenings => Set<Screening>();
        public DbSet<Seat> Seats => Set<Seat>();

        public DbSet<Booking> Bookings => Set<Booking>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Seat>().HasIndex(s => new { s.ScreeningId, s.SeatNumber }).IsUnique();

            modelBuilder.Entity<Booking>()
            .HasOne(b => b.Seat)
            .WithMany()
            .HasForeignKey(b => b.SeatId)
            .OnDelete(DeleteBehavior.Restrict);
                }
    }
}
