using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using TicketBooking.Application.Dtos;
using TicketBooking.Core.Entities;
using TicketBooking.Core.Exceptions;
using TicketBooking.Core.Interfaces;
using TicketBooking.Infrastructure.Data;

namespace TicketBooking.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly ILogger<BookingService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public BookingService(ApplicationDbContext context, ILogger<BookingService> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<bool> BookSeatsAsync(AddBookingDto bookingDto)
        {
            _logger.LogInformation("Booking started for screening {ScreeningId} with seats: {Seats}",
            bookingDto.ScreeningId, string.Join(", ", bookingDto.SeatNumbers));

            Screening? screening;
            var cacheKey = $"screening:{bookingDto.ScreeningId}";
            try
            {
                var cachedScreening = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedScreening))
                {
                    screening = JsonSerializer.Deserialize<Screening>(cachedScreening, new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.IgnoreCycles,
                        PropertyNameCaseInsensitive = true
                    })!;
                    _logger.LogInformation("Screening loaded from Redis cache.");
                }
                else
                {
                    screening = await _context.Screenings
                        .Include(s => s.Seats)
                        .FirstOrDefaultAsync(s => s.Id == bookingDto.ScreeningId);

                    if (screening == null)
                    {
                        _logger.LogWarning("Screening not found for ID: {ScreeningId}", bookingDto.ScreeningId);
                        return false;
                    }

                    var serialized = JsonSerializer.Serialize(screening, new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.IgnoreCycles,
                        WriteIndented = false
                    });

                    await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    });

                    _logger.LogInformation("Screening cached to Redis.");
                }

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

                await _cache.RemoveAsync(cacheKey);
                _logger.LogInformation("Cache invalidated for screening {ScreeningId}", bookingDto.ScreeningId);

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
