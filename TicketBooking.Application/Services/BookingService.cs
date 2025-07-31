using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TicketBooking.Application.Dtos;
using TicketBooking.Core.Exceptions;
using TicketBooking.Core.Interfaces;
using TicketBooking.Infrastructure.Data;

namespace TicketBooking.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly ILogger<BookingService> _logger;
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context, ILogger<BookingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> BookSeatsAsync(AddBookingDto bookingDto)
        {
            _logger.LogInformation("Booking started for screening {ScreeningId} with seats: {Seats}",
            bookingDto.ScreeningId, string.Join(", ", bookingDto.SeatNumbers));

            try
            {
                var screening = await _context.Screenings
                    .Include(s => s.Seats)
                    .FirstOrDefaultAsync(s => s.Id == bookingDto.ScreeningId);

                if (screening == null) return false;

                var targetSeats = screening.Seats
                   .Where(seat => !string.IsNullOrWhiteSpace(seat.SeatNumber)
                   && bookingDto.SeatNumbers.Contains(seat.SeatNumber))
                   .ToList();

                if(targetSeats.Any(x => x.IsBooked))
                {
                    //Seats are taken
                    _logger.LogWarning("No available seats found for booking for screening {ScreeningId} with seats: {Seats}",
                        bookingDto.ScreeningId, string.Join(", ", bookingDto.SeatNumbers));
                    IEnumerable<string> seatNumber = targetSeats.Where(x => x.IsBooked).Select(x => x.SeatNumber);
                    throw new SeatAlreadyBookedException(seatNumber);
                }

                if(targetSeats.Count == 0)
                {
                    //Seats does not exist
                    _logger.LogWarning("Seats not found for booking for screening {ScreeningId} with seats: {Seats}",
                        bookingDto.ScreeningId, string.Join(", ", bookingDto.SeatNumbers));
                    throw new AppException("Seats not found for booking", 404);
                }

                foreach (var seat in targetSeats)
                {
                    seat.IsBooked = true;
                }

                var booking = new Core.Entities.Booking
                {
                    NameOfPerson = bookingDto.NameOfPerson,
                    ScreeningId = bookingDto.ScreeningId,
                    BookedSeats = targetSeats.Select(s => s.SeatNumber).ToList(),
                    BookedAt = DateTime.UtcNow
                };

                _context.Bookings.Add(booking);

                await _context.SaveChangesAsync();
                return true;

            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, "Booking failed due to: {Message}", ex.Message);
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict while booking seats for screening {ScreeningId}", bookingDto.ScreeningId);
                throw new AppException("Seat booking failed due to a concurrency issue. Please try again.", 409);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in BookSeatsAsync for screening {ScreeningId}", bookingDto.ScreeningId);
                throw;
            }
        }
    }
}
