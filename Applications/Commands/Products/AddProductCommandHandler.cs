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
    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, string>
    {
        private readonly AppDbContext _context;

        public AddProductCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var warehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Name == request.WarehouseName);
            if (warehouse == null)
            {
                throw new Exception($"Warehouse '{request.WarehouseName}' not found");
            }

            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Name == request.Name && !p.IsDeleted);
            if (existingProduct != null)
            {
                throw new Exception("Product with the same name already exists");
            }

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                LowStockThreshold = request.LowStockThreshold,
                Category = request.Category,
                IsDeleted = false
            };

            var productWarehouse = new ProductWarehouse
            {
                Product = product,
                WarehouseId = warehouse.Id,
                Quantity = request.Quantity
            };

            _context.Products.Add(product);
            _context.ProductWarehouses.Add(productWarehouse);
            await _context.SaveChangesAsync();

            return "Product added successfully";
        }
    }

}
