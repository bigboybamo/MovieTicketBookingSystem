using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        public async Task<bool> BookSeatsAsync(int screeningId, List<string> seatNumbers)
        {
            _logger.LogInformation("Booking started for screening {ScreeningId} with seats: {Seats}",
            screeningId, string.Join(", ", seatNumbers));

            try
            {
                var screening = await _context.Screenings
                    .Include(s => s.Seats)
                    .FirstOrDefaultAsync(s => s.Id == screeningId);

                if (screening == null) return false;

                var targetSeats = screening.Seats
                   .Where(seat => !string.IsNullOrWhiteSpace(seat.SeatNumber)
                   && seatNumbers.Contains(seat.SeatNumber)
                   && !seat.IsBooked)
                   .ToList();

                if (targetSeats.Count != seatNumbers.Count)
                {
                     var unavailable = seatNumbers
                    .Except(targetSeats
                        .Where(s => !string.IsNullOrWhiteSpace(s.SeatNumber))
                        .Select(s => s.SeatNumber))
                    .ToList();

                    if (!unavailable.Any())
                    {
                        _logger.LogWarning("Booking failed: some requested seats not found, but unavailable list was empty.");
                        throw new AppException("Some requested seats are not available.", 409);
                    }
                }

                foreach (var seat in targetSeats)
                {
                    seat.IsBooked = true;
                }

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
                _logger.LogError(ex, "Concurrency conflict while booking seats for screening {ScreeningId}", screeningId);
                throw new AppException("Seat booking failed due to a concurrency issue. Please try again.", 409);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in BookSeatsAsync for screening {ScreeningId}", screeningId);
                throw;
            }
        }
    }
}
