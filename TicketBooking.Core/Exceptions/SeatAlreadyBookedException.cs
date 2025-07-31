using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketBooking.Core.Exceptions
{
    public class SeatAlreadyBookedException : AppException
    {
        public IEnumerable<string> SeatNumbers { get; }

        public SeatAlreadyBookedException(IEnumerable<string> seatNumbers)
            : base($"The following seats are already booked: {string.Join(", ", seatNumbers)}", 409)
        {
            SeatNumbers = seatNumbers;
        }
    }
}
