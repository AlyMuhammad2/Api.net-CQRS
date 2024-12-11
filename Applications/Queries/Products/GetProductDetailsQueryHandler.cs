using Infrastructue.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Queries.Products
{
    public class GetProductDetailsQueryHandler : IRequestHandler<GetProductDetailsQuery, ProductDto>
    {
        private readonly AppDbContext _context;

        public GetProductDetailsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductDto> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(p => p.ProductWarehouses)
                .ThenInclude(pw => pw.Warehouse)
                .Where(p => p.Id == request.ProductId && !p.IsDeleted)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    LowStockThreshold = p.LowStockThreshold,
                    Category = p.Category,
                    TotalQuantity = p.ProductWarehouses.Sum(pw => pw.Quantity),
                    Warehouses = p.ProductWarehouses.Select(pw => new WarehouseDto
                    {
                        Name = pw.Warehouse.Name,
                        Quantity = pw.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            return product;
        }
    }

}
