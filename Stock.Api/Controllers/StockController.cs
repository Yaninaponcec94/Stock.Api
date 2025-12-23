using Microsoft.AspNetCore.Authorization;
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

		[HttpGet]
		public async Task<IActionResult> GetStock()
		{
			var stock = await _service.GetStockAsync();
			return Ok(stock);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost("entry")]
		public async Task<IActionResult> Entry([FromBody] CreateStockMovementDto dto)
		{
			dto.Type = "Entry";
			var result = await _service.CreateMovementAsync(dto.ProductId, dto.Type, dto.Quantity, dto.Reason);
			return Ok(result);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost("exit")]
		public async Task<IActionResult> Exit([FromBody] CreateStockMovementDto dto)
		{
			dto.Type = "Exit";
			var result = await _service.CreateMovementAsync(dto.ProductId, dto.Type, dto.Quantity, dto.Reason);
			return Ok(result);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost("adjustment")]
		public async Task<IActionResult> Adjustment([FromBody] CreateStockMovementDto dto)
		{
			dto.Type = "Adjustment";
			var result = await _service.CreateMovementAsync(dto.ProductId, dto.Type, dto.Quantity, dto.Reason);
			return Ok(result);
		}
	}
}

