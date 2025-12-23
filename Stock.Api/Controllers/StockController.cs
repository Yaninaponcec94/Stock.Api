using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock.Application.DTOs;
using Stock.Application.Interfaces;
using Stock.Application.Models;

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
			var result = await _service.CreateMovementAsync(dto.ProductId, StockMovementType.Entry, dto.Quantity, dto.Reason);
			return Ok(result);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost("exit")]
		public async Task<IActionResult> Exit([FromBody] CreateStockMovementDto dto)
		{
			var result = await _service.CreateMovementAsync(dto.ProductId, StockMovementType.Exit, dto.Quantity, dto.Reason);
			return Ok(result);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost("adjustment")]
		public async Task<IActionResult> Adjustment([FromBody] CreateStockMovementDto dto)
		{
			var result = await _service.CreateMovementAsync(dto.ProductId, StockMovementType.Adjustment, dto.Quantity, dto.Reason);
			return Ok(result);
		}
	}
}
