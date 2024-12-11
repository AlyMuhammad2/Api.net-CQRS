using Applications.Commands.Inventory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaskCQRS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("add-stock")]
        [Authorize]
        public async Task<IActionResult> AddStock(int productId, string warehouseName, int quantity)
        {
            try
            {
                var performedBy = User.Identity?.Name;
                if (string.IsNullOrEmpty(performedBy))
                {
                    return Unauthorized("User is not authenticated");
                }

                var command = new StockCommand
                {
                    ProductId = productId,
                    WarehouseName = warehouseName,
                    Quantity = quantity,
                    PerformedBy = performedBy
                };

                var result = await _mediator.Send(command);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPost("remove-stock")]
        [Authorize]
        public async Task<IActionResult> RemoveStock(int productId, string warehouseName, int quantity)
        {
            try
            {
                var performedBy = User.Identity?.Name;
                if (string.IsNullOrEmpty(performedBy))
                {
                    return Unauthorized("User is not authenticated");
                }

                var command = new RemoveStockCommand
                {
                    ProductId = productId,
                    WarehouseName = warehouseName,
                    Quantity = quantity,
                    PerformedBy = performedBy
                };

                var result = await _mediator.Send(command);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPost("transfer-stock")]
        [Authorize]
        public async Task<IActionResult> TransferStock(int productId, int quantity, string sourceWarehouseName, string targetWarehouseName)
        {
            try
            {
                var performedBy = User.Identity?.Name;
                if (string.IsNullOrEmpty(performedBy))
                {
                    return Unauthorized("User is not authenticated");
                }

                var command = new TransferStockCommand
                {
                    ProductId = productId,
                    Quantity = quantity,
                    SourceWarehouseName = sourceWarehouseName,
                    TargetWarehouseName = targetWarehouseName,
                    PerformedBy = performedBy
                };

                var result = await _mediator.Send(command);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


    }
}
