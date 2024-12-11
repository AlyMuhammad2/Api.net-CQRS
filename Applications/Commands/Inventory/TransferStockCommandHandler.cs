using Domain.Entities;
using Infrastructue.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Commands.Inventory
{
    public class TransferStockCommandHandler : IRequestHandler<TransferStockCommand, string>
    {
        private readonly AppDbContext _context;

        public TransferStockCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(TransferStockCommand request, CancellationToken cancellationToken)
        {
            var sourceWarehouse = await _context.Warehouses
                .Include(w => w.ProductWarehouses)
                .FirstOrDefaultAsync(w => w.Name == request.SourceWarehouseName);

            var targetWarehouse = await _context.Warehouses
              .Include(w => w.ProductWarehouses)
              .FirstOrDefaultAsync(w => w.Name == request.TargetWarehouseName);

            if (sourceWarehouse == null)
            {
                throw new Exception($"Source warehouse '{request.SourceWarehouseName}' not found");
            }

          

            if (targetWarehouse == null)
            {
                throw new Exception($"Target warehouse '{request.TargetWarehouseName}' not found");
            }

            var sourceProductWarehouse = sourceWarehouse.ProductWarehouses
                .FirstOrDefault(pw => pw.ProductId == request.ProductId);

            if (sourceProductWarehouse == null || sourceProductWarehouse.Quantity < request.Quantity)
            {
                throw new Exception($"Insufficient stock in source warehouse '{request.SourceWarehouseName}' or product not found");
            }

            var targetProductWarehouse = targetWarehouse.ProductWarehouses
                .FirstOrDefault(pw => pw.ProductId == request.ProductId);

            if (targetProductWarehouse == null)
            {
                targetProductWarehouse = new ProductWarehouse
                {
                    ProductId = request.ProductId,
                    WarehouseId = targetWarehouse.Id,
                    Quantity = 0
                };

                _context.ProductWarehouses.Add(targetProductWarehouse);
            }

            sourceProductWarehouse.Quantity -= request.Quantity;
            targetProductWarehouse.Quantity += request.Quantity;

            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                ProductId = request.ProductId,
                TransactionType = "Transfer",
                SourceWarehouse = request.SourceWarehouseName,
                TargetWarehouse = request.TargetWarehouseName,
                Quantity = request.Quantity,
                PerformedBy = request.PerformedBy,
                Date = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return $"Successfully transferred {request.Quantity} units of product ID {request.ProductId} from '{request.SourceWarehouseName}' to '{request.TargetWarehouseName}'";
        }
    }

}
