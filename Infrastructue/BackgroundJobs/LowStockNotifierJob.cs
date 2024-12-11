using Hangfire;
using Infrastructue.Notifications;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructue.BackgroundJobs
{
    public class LowStockNotifierJob
    {
        private readonly IMediator _mediator;

        public LowStockNotifierJob(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task Execute(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new NotifyLowStockQuery(), cancellationToken);
        }
    }

}
