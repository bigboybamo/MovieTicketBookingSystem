using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketBooking.Core.Interfaces
{
    public interface IBookingService
    {
        Task<bool> BookSeatsAsync(int screeningId, List<string> seatNumbers);
    }
}
