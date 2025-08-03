using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;
using TicketBooking.Application.Dtos;
using TicketBooking.Application.Services;
using TicketBooking.Core.Exceptions;
using TicketBooking.Core.Interfaces;
using TicketBooking.Infrastructure.Data;
using TicketBooking.Tests.Helpers;

namespace TicketBooking.Tests.ServiceTests
{
    public class Tests
    {
        private ApplicationDbContext _dbContext;
        private Mock<ILogger<BookingService>> _logger;
        private BookingService _bookingService;

        [SetUp]
        public void Setup()
        {
            _dbContext = TestHelper.GetInMemoryDbContextAsync();
            _logger = new Mock<ILogger<BookingService>>();
            _bookingService = new BookingService(_dbContext, _logger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext?.Dispose();
        }

        [Test]
        public async Task BookSeatsAsync_Should_BookSeats_Successfully()
        {
            var dto = new AddBookingDto
            {
                NameOfPerson = "John Doe",
                ScreeningId = 1,
                SeatNumbers = new List<string> { "A1", "A3" }
            };

            var result = await _bookingService.BookSeatsAsync(dto);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task BookSeatsAsync_Should_Throw_SeatAlreadyBookedException_If_Seat_Already_Booked()
        {
            var dto = new AddBookingDto
            {
                NameOfPerson = "Jane Smith",
                ScreeningId = 1,
                SeatNumbers = new List<string> { "A2" } // already booked
            };

            var ex = Assert.ThrowsAsync<SeatAlreadyBookedException>(() => _bookingService.BookSeatsAsync(dto));

            Assert.Contains("A2", ex.SeatNumbers.ToList());
        }

        [Test]
        public async Task BookSeatsAsync_Should_Throw_AppException_If_Seat_Does_Not_Exist()
        {
            var dto = new AddBookingDto
            {
                NameOfPerson = "Alice",
                ScreeningId = 1,
                SeatNumbers = new List<string> { "B99" } // non-existent seat
            };

            var ex = Assert.ThrowsAsync<AppException>(() => _bookingService.BookSeatsAsync(dto));
            Assert.That(ex.StatusCode, Is.EqualTo(404));
        }
    }
}