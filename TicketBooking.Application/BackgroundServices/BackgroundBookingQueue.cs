using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using TicketBooking.Application.Dtos;

namespace TicketBooking.Application.BackgroundServices
{
    public class BackgroundBookingQueue : IBackgroundBookingQueue
    {
        private readonly Channel<AddBookingDto> _queue;

        public BackgroundBookingQueue()
        {
            _queue = Channel.CreateBounded<AddBookingDto>(100);
        }

        public void QueueBooking(AddBookingDto request)
        {
            if (!_queue.Writer.TryWrite(request))
            {
                throw new InvalidOperationException("Booking queue is full.");
            }
        }

        public async IAsyncEnumerable<AddBookingDto> DequeueAsync(CancellationToken cancellationToken)
        {
            while (await _queue.Reader.WaitToReadAsync(cancellationToken))
            {
                while (_queue.Reader.TryRead(out var item))
                {
                    yield return item;
                }
            }
        }
    }
}
