using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBooking.Core.Interfaces;

namespace TicketBooking.Application.BackgroundServices
{
    public class BackgroundBookingWorker : BackgroundService
    {
        private readonly IBackgroundBookingQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BackgroundBookingWorker> _logger;

        public BackgroundBookingWorker(
            IBackgroundBookingQueue queue,
            IServiceScopeFactory scopeFactory,
            ILogger<BackgroundBookingWorker> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BackgroundBookingWorker started.");

            await foreach (var bookingRequest in _queue.DequeueAsync(stoppingToken))
            {
                using var scope = _scopeFactory.CreateScope();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

                try
                {
                    await bookingService.BookSeatsAsync(bookingRequest);
                    _logger.LogInformation("Processed booking for screening {ScreeningId}", bookingRequest.ScreeningId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process booking for screening {ScreeningId}", bookingRequest.ScreeningId);
                }
            }

            _logger.LogInformation("BackgroundBookingWorker stopped.");
        }
    }
}
