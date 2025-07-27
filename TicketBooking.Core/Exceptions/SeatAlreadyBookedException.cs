using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketBooking.Core.Exceptions
{
    public class SeatAlreadyBookedException : AppException
    {
        public SeatAlreadyBookedException(string seatNumber)
            : base($"Seat '{seatNumber}' is already booked.", 409) { }
    }
}
