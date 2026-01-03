using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock.Api.DTOs;
using Stock.Application.Interfaces;
using Stock.Application.Models;

namespace Stock.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class StockController : ControllerBase
	{
		private readonly IStockService _service;

		public StockController(IStockService service) => _service = service;

		[HttpGet]
		public async Task<IActionResult> GetStock()
			=> Ok(await _service.GetStockAsync());

		[HttpPost("entry")]
		public async Task<IActionResult> Entry([FromBody] CreateStockMovementRequestDto dto)
			=> Ok(await _service.CreateMovementAsync(dto.ProductId, StockMovementType.Entry, dto.Quantity, dto.Reason));

		[HttpPost("exit")]
		public async Task<IActionResult> Exit([FromBody] CreateStockMovementRequestDto dto)
			=> Ok(await _service.CreateMovementAsync(dto.ProductId, StockMovementType.Exit, dto.Quantity, dto.Reason));


		[Authorize(Roles = "Admin")]
		[HttpPost("adjustment")]
		public async Task<IActionResult> Adjustment(
		[FromBody] CreateStockAdjustmentRequestDto dto
		)
				{
					var result = await _service.CreateMovementAsync(
						dto.ProductId,
						StockMovementType.Adjustment,
						dto.Quantity,
						dto.Reason
					);

					return Ok(result);
				}

		[HttpGet("movements")]
		public async Task<IActionResult> GetMovements([FromQuery] int? productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
			=> Ok(await _service.GetMovementsAsync(productId, page, pageSize));
	}
}
