using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketBooking.Core.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        public int ScreeningId { get; set; }
        public Screening Screening { get; set; }

        public int SeatId { get; set; }
        public Seat Seat { get; set; }

        public string NameOfPerson { get; set; }

        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    }

}
