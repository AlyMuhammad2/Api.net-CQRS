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
    public class RemoveStockCommandHandler : IRequestHandler<RemoveStockCommand, string>
    {
        private readonly AppDbContext _context;

        public RemoveStockCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(RemoveStockCommand request, CancellationToken cancellationToken)
        {
            var productWarehouse = await _context.ProductWarehouses
                .Include(pw => pw.Product)
                .Include(pw => pw.Warehouse)
                .FirstOrDefaultAsync(pw => pw.ProductId == request.ProductId && pw.Warehouse.Name == request.WarehouseName);

            if (productWarehouse == null)
            {
                throw new Exception("Product not found");
            }

            if (productWarehouse.Quantity < request.Quantity)
            {
                throw new Exception("Insufficient stock");
            }

            productWarehouse.Quantity -= request.Quantity;

            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                ProductId = request.ProductId,
                TransactionType = "Remove",
                Quantity = request.Quantity,
                PerformedBy = request.PerformedBy,
                SourceWarehouse = request.WarehouseName,
                Date = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return "Stock removed successfully";
        }

        
    }

}
