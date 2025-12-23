using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock.Api.DTOs;
using Stock.Application.Exceptions;
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

		[Authorize(Roles = "Admin")] //solo admin puede crear movimientos
		[HttpPost("movements")]
		public async Task<IActionResult> CreateMovement([FromBody] CreateStockMovementDto dto)
		{
			try
			{
				var result = await _service.CreateMovementAsync(dto.ProductId, dto.Type, dto.Quantity, dto.Reason);
				return Ok(result);
			}
			catch (ConcurrencyException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpGet("alerts")]
		public async Task<IActionResult> GetAlerts()
		{
			var alerts = await _service.GetStockAlertsAsync();
			return Ok(alerts);
		}

	}
}
