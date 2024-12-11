using Hangfire;
using Infrastructue.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructue.BackgroundJobs
{
    public class TransactionJob
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TransactionJob> _logger;

        public TransactionJob(IMediator mediator, ILogger<TransactionJob> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task Execute()
        {
            var oneYearAgo = DateTime.UtcNow.AddYears(-1);
            _logger.LogInformation("Starting archive job for transactions older than {ArchiveDate}.", oneYearAgo);

            await _mediator.Send(new NotifyArchivedTransactions(oneYearAgo));
        }
    }
}
