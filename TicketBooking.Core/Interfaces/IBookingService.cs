using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBooking.Application.Dtos;

namespace TicketBooking.Core.Interfaces
{
    public interface IBookingService
    {
        Task<bool> BookSeatsAsync(AddBookingDto bookingDto);
    }
}
