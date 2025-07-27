using Microsoft.AspNetCore.Mvc;
using TicketBooking.Core.Interfaces;

namespace TicketBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("{screeningId}/book")]
        public async Task<IActionResult> Book(int screeningId, [FromBody] List<string> seatNumbers)
        {
            var success = await _bookingService.BookSeatsAsync(screeningId, seatNumbers);
            return success ? Ok("Booking successful") : BadRequest("Booking failed");
        }
    }

}
