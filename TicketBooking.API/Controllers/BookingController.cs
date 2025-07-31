using Microsoft.AspNetCore.Mvc;
using TicketBooking.Application.Dtos;
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

        [HttpPost]
        [Route("AddBooking")]
        public async Task<IActionResult> Book(AddBookingDto bookingDto)
        {
            var success = await _bookingService.BookSeatsAsync(bookingDto);
            return success ? Ok("Booking successful") : BadRequest("Booking failed");
        }
    }

}
