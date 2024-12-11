using Applications.Commands.Products;
using Applications.Queries.Products;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaskCQRS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListProducts()
        {
            try
            {
                var result = await _mediator.Send(new ListProductsQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductDetails(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetProductDetailsQuery { ProductId = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromBody] AddProductCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid values for product ");
            }
            try
            {
                var result = await _mediator.Send(command);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductCommand command)
        {
            try
            {
                command.ProductId = id;
                var result = await _mediator.Send(command);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteProductCommand { ProductId = id });
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

    }
}
