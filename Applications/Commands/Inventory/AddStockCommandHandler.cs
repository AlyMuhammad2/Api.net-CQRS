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
    public class AddStockCommandHandler : IRequestHandler<StockCommand, string>
    {
        private readonly AppDbContext _context;

        public AddStockCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(StockCommand request, CancellationToken cancellationToken)
        {
            var productWarehouse = await _context.ProductWarehouses
                .Include(pw => pw.Product)
                .Include(pw => pw.Warehouse)
                .FirstOrDefaultAsync(pw => pw.Product.Id == request.ProductId && pw.Warehouse.Name == request.WarehouseName);

            if (productWarehouse == null)
            {
                throw new Exception("Product or Warehouse not found");
            }

            productWarehouse.Quantity += request.Quantity;

            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                ProductId = request.ProductId,
                TransactionType = "Add",
                Quantity = request.Quantity,
                PerformedBy = request.PerformedBy,
                SourceWarehouse = request.WarehouseName,
                Date = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return "Stock added successfully";
        }
    }
}