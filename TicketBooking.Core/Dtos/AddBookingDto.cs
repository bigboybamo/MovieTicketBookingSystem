using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketBooking.Application.Dtos
{
    public class AddBookingDto
    {
        public string NameOfPerson { get; set; } = string.Empty;
        public int ScreeningId { get; set; }
        public List<string> SeatNumbers { get; set; } = new List<string>();
    }
}
