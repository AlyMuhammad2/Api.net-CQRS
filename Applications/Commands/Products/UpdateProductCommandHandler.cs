using Domain.Entities;
using Infrastructue.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Commands.Products
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, string>
    {
        private readonly AppDbContext _context;
        public UpdateProductCommandHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<string> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                                .Include(p=>p.ProductWarehouses)
                                .FirstOrDefaultAsync(p=>p.Id == request.ProductId);
            if (product == null || product.IsDeleted) {
                throw new Exception("Product not found");
            }
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.LowStockThreshold = request.LowStockThreshold;
            product.Category = request.Category;

            if (!string.IsNullOrEmpty(request.WarehouseName))
            {
                var warehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Name == request.WarehouseName);
                if (warehouse == null)
                {
                    throw new Exception($"Warehouse '{request.WarehouseName}' not found");
                }

                var productWarehouse = product.ProductWarehouses.FirstOrDefault(pw => pw.WarehouseId == warehouse.Id);

                if (productWarehouse == null)
                {
                    productWarehouse = new ProductWarehouse
                    {
                        ProductId = product.Id,
                        WarehouseId = warehouse.Id,
                        Quantity = request.Quantity
                    };
                    _context.ProductWarehouses.Add(productWarehouse);
                }
                else
                {
                    productWarehouse.Quantity = request.Quantity;
                }
            }

            await _context.SaveChangesAsync();
            return "Product updated successfully";
        }
    
    }
}
