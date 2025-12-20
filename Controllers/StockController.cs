using Microsoft.AspNetCore.Mvc;
using Stock.Api.DTOs;
using Stock.Application.Interfaces;

namespace Stock.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class StockController : ControllerBase
	{
		private readonly IStockService _service;

		public StockController(IStockService service)
		{
			_service = service;
		}

		[HttpPost("movements")]
		public async Task<IActionResult> CreateMovement([FromBody] CreateStockMovementDto dto)
		{
			try
			{
				var result = await _service.CreateMovementAsync(dto.ProductId, dto.Type, dto.Quantity, dto.Reason);
				return Ok(result);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}
