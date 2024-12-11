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
    public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, TransactionHistoryResult>
    {
        private readonly AppDbContext _context;

        public GetTransactionHistoryQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TransactionHistoryResult> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            if (request.CurrentPage < 1 || request.PageSize < 1)
            {
                throw new Exception("CurrentPage and PageSize must be greater than 0");
            }

            var query = _context.InventoryTransactions
                .Include(t => t.product)
                .AsQueryable();

            if (request.ProductId.HasValue)
                query = query.Where(t => t.ProductId == request.ProductId.Value);

            if (request.StartDate.HasValue && request.EndDate.HasValue)
                query = query.Where(t => t.Date >= request.StartDate.Value && t.Date <= request.EndDate.Value);

            if (!string.IsNullOrEmpty(request.TransactionType))
                query = query.Where(t => t.TransactionType != null && t.TransactionType.ToLower() == request.TransactionType.ToLower());

            if (!string.IsNullOrEmpty(request.Category))
                query = query.Where(t => t.product.Category != null && t.product.Category.ToLower() == request.Category.ToLower());

            var totalItems = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

            if (request.CurrentPage > totalPages && totalPages > 0)
            {
                throw new Exception("No data available for the requested page");
            }

            var transactions = await query.OrderBy(t => t.Date)
                .Skip((request.CurrentPage - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.Id,
                    ProductName = t.product.Name,
                    TransactionType = t.TransactionType,
                    Quantity = t.Quantity,
                    Date = t.Date,
                    Category = t.product.Category,
                    PerformedBy = t.PerformedBy
                })
                .ToListAsync(cancellationToken);

            var nextPage = request.CurrentPage < totalPages
                ? $"/api/report/transaction-history?currentPage={request.CurrentPage + 1}&pageSize={request.PageSize}"
                : null;

            var previousPage = request.CurrentPage > 1
                ? $"/api/report/transaction-history?currentPage={request.CurrentPage - 1}&pageSize={request.PageSize}"
                : null;

            return new TransactionHistoryResult
            {
                Data = transactions,
                TotalItems = totalItems,
                TotalPages = totalPages,
                NextPage = nextPage,
                PreviousPage = previousPage
            };
        }
    }

}
