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
    public class NotifyLowStockQueryHandler : IRequestHandler<NotifyLowStockQuery>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<NotifyLowStockQueryHandler> _logger;

        public NotifyLowStockQueryHandler(AppDbContext context, ILogger<NotifyLowStockQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(NotifyLowStockQuery request, CancellationToken cancellationToken)
        {
            var lowStockProducts = await _context.ProductWarehouses
                .Include(pw => pw.Product)
                .Include(pw => pw.Warehouse)
                .Where(pw => pw.Quantity <= pw.Product.LowStockThreshold)
                .ToListAsync(cancellationToken);

            if (!lowStockProducts.Any())
            {
                _logger.LogInformation("No low stock products found");
                return Unit.Value;
            }

            foreach (var product in lowStockProducts)
            {
                _logger.LogWarning($"Low stock alert: Product '{product.Product.Name}' in Warehouse '{product.Warehouse.Name}' has only {product.Quantity} left (Threshold: {product.Product.LowStockThreshold})");
            }

            return Unit.Value;
        }
    }

}
