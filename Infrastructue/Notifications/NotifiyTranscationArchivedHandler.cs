using Domain.Entities;
using Infrastructue.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructue.Notifications
{
    public class NotifyArchivedTransactionsHandler : IRequestHandler<NotifyArchivedTransactions>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<NotifyArchivedTransactionsHandler> _logger;

        public NotifyArchivedTransactionsHandler(AppDbContext context, ILogger<NotifyArchivedTransactionsHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(NotifyArchivedTransactions request, CancellationToken cancellationToken)
        {
            var oldTransactions = await _context.InventoryTransactions
                .Where(t => t.Date <= request.ArchiveDate)
                .ToListAsync(cancellationToken);

            if (!oldTransactions.Any())
            {
                _logger.LogInformation("No transactions found for archiving.");
                return Unit.Value;
            }

            var archivedTransactions = oldTransactions.Select(t => new ArchivedTransaction
            {
                TransactionType = t.TransactionType,
                ProductId = t.ProductId,
                Quantity = t.Quantity,
                Date = t.Date,
                SourceWarehouse = t.SourceWarehouse,
                TargetWarehouse = t.TargetWarehouse,
                PerformedBy = t.PerformedBy
            }).ToList();

            await _context.ArchivedTransactions.AddRangeAsync(archivedTransactions, cancellationToken);
            _context.InventoryTransactions.RemoveRange(oldTransactions);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"{archivedTransactions.Count} transactions have been archived.");

            return Unit.Value;
        }

    }
}