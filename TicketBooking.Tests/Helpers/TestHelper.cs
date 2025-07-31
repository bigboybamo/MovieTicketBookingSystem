using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBooking.Core.Entities;
using TicketBooking.Infrastructure.Data;

namespace TicketBooking.Tests.Helpers
{
    public static class TestHelper
    {
        public static ApplicationDbContext GetInMemoryDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            var screening = new Screening
            {
                Id = 1,
                MovieId = 1,
                StartTime = DateTime.UtcNow,
                Seats = new List<Seat>
            {
                new Seat { Id = 1, SeatNumber = "A1", IsBooked = false },
                new Seat { Id = 2, SeatNumber = "A2", IsBooked = true },
                new Seat { Id = 3, SeatNumber = "A3", IsBooked = false },
            }
            };

            context.Screenings.Add(screening);
            context.SaveChangesAsync();

            return context;
        }
    }
}
