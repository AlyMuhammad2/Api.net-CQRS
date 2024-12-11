using Applications.Queries.Report;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaskCQRS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockReport([FromQuery] int currentPage = 1, [FromQuery] int pageSize = 3)
        {
            try
            {
                var query = new GetLowStockReportQuery
                {
                    CurrentPage = currentPage,
                    PageSize = pageSize
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("transaction-history")]
        public async Task<IActionResult> GetTransactionHistory(
            int? productId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? transactionType,
            [FromQuery] string? category,
            [FromQuery] int currentPage = 1,
            [FromQuery] int pageSize = 3)
        {
            try
            {
                var query = new GetTransactionHistoryQuery
                {
                    ProductId = productId,
                    StartDate = startDate,
                    EndDate = endDate,
                    TransactionType = transactionType,
                    Category = category,
                    CurrentPage = currentPage,
                    PageSize = pageSize
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}

