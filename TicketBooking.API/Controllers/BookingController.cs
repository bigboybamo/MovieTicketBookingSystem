using Microsoft.AspNetCore.Mvc;
using TicketBooking.Application.BackgroundServices;
using TicketBooking.Application.Dtos;
using TicketBooking.Core.Interfaces;

namespace TicketBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IBackgroundBookingQueue _queue;

        public BookingController(IBookingService bookingService, IBackgroundBookingQueue queue)
        {
            _bookingService = bookingService;
            _queue = queue;
        }

        [HttpPost]
        [Route("AddBooking")]
        public async Task<IActionResult> Book(AddBookingDto bookingDto)
        {
            var success = await _bookingService.BookSeatsAsync(bookingDto);
            return success ? Ok("Booking successful") : BadRequest("Booking failed");
        }

        [HttpPost]
        [Route("queue-booking")]
        public IActionResult QueueBooking([FromBody] AddBookingDto request)
        {
            _queue.QueueBooking(request);
            return Ok("Booking has been queued successfully");
        }
    }

}
