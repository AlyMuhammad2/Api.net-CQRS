using Infrastructue.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Queries.Report
{
    public class GetLowStockReportQueryHandler : IRequestHandler<GetLowStockReportQuery, LowStockReportResult>
    {
        private readonly AppDbContext _context;

        public GetLowStockReportQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LowStockReportResult> Handle(GetLowStockReportQuery request, CancellationToken cancellationToken)
        {
            if (request.CurrentPage < 1 || request.PageSize < 1)
            {
                throw new Exception("CurrentPage and PageSize must be greater than 0");
            }

            var query = _context.Products
                .Include(p => p.ProductWarehouses)
                .ThenInclude(pw => pw.Warehouse)
                .Where(p => p.ProductWarehouses.Any(pw => pw.Quantity <= p.LowStockThreshold) && !p.IsDeleted);

            var totalItems = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

            if (request.CurrentPage > totalPages && totalPages > 0)
            {
                throw new Exception("No data available for the requested page");
            }

            var lowStockProducts = await query.OrderBy(p => p.Name)
                .Skip((request.CurrentPage - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    LowStockThreshold = p.LowStockThreshold,
                    Price = p.Price,
                    Category = p.Category,
                    TotalQuantity = p.ProductWarehouses.Sum(pw => pw.Quantity),
                    Warehouses = p.ProductWarehouses
                        .Where(pw => pw.Quantity < p.LowStockThreshold)
                        .Select(pw => new WarehouseDto
                        {
                            Name = pw.Warehouse.Name,
                            Quantity = pw.Quantity
                        }).ToList()
                })
                .ToListAsync(cancellationToken);

            var nextPage = request.CurrentPage < totalPages
                ? $"/api/report/low-stock?currentPage={request.CurrentPage + 1}&pageSize={request.PageSize}"
                : null;

            var previousPage = request.CurrentPage > 1
                ? $"/api/report/low-stock?currentPage={request.CurrentPage - 1}&pageSize={request.PageSize}"
                : null;

            return new LowStockReportResult
            {
                Data = lowStockProducts,
                TotalItems = totalItems,
                TotalPages = totalPages,
                NextPage = nextPage,
                PreviousPage = previousPage
            };
        }
    }


}
