using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBooking.Application.Dtos;

namespace TicketBooking.Application.BackgroundServices
{
    public interface IBackgroundBookingQueue
    {
        void QueueBooking(AddBookingDto request);
        IAsyncEnumerable<AddBookingDto> DequeueAsync(CancellationToken cancellationToken);
    }
}
