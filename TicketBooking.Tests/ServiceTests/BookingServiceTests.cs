using Microsoft.Extensions.Logging;
using Moq;
using TicketBooking.Application.Dtos;
using TicketBooking.Application.Services;
using TicketBooking.Core.Exceptions;
using TicketBooking.Infrastructure.Data;
using TicketBooking.Tests.Helpers;

namespace TicketBooking.Tests.ServiceTests
{
    public class Tests
    {
        private ApplicationDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            _dbContext = TestHelper.GetInMemoryDbContextAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext?.Dispose();
        }

        [Test]
        public async Task BookSeatsAsync_Should_BookSeats_Successfully()
        {
            var logger = new Mock<ILogger<BookingService>>();
            var service = new BookingService(_dbContext, logger.Object);

            var dto = new AddBookingDto
            {
                NameOfPerson = "John Doe",
                ScreeningId = 1,
                SeatNumbers = new List<string> { "A1", "A3" }
            };

            var result = await service.BookSeatsAsync(dto);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task BookSeatsAsync_Should_Throw_SeatAlreadyBookedException_If_Seat_Already_Booked()
        {
            var logger = new Mock<ILogger<BookingService>>();
            var service = new BookingService(_dbContext, logger.Object);

            var dto = new AddBookingDto
            {
                NameOfPerson = "Jane Smith",
                ScreeningId = 1,
                SeatNumbers = new List<string> { "A2" } // already booked
            };

            var ex = Assert.ThrowsAsync<SeatAlreadyBookedException>(() => service.BookSeatsAsync(dto));

            Assert.Contains("A2", ex.SeatNumbers.ToList());
        }

        [Test]
        public async Task BookSeatsAsync_Should_Throw_AppException_If_Seat_Does_Not_Exist()
        {
            var logger = new Mock<ILogger<BookingService>>();
            var service = new BookingService(_dbContext, logger.Object);

            var dto = new AddBookingDto
            {
                NameOfPerson = "Alice",
                ScreeningId = 1,
                SeatNumbers = new List<string> { "B99" } // non-existent seat
            };

            var ex = Assert.ThrowsAsync<AppException>(() => service.BookSeatsAsync(dto));
            Assert.That(ex.StatusCode, Is.EqualTo(404));
        }
    }
}