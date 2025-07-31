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
        
        public List<string> BookedSeats { get; set; } = new List<string>();

        public string NameOfPerson { get; set; }

        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    }

}
