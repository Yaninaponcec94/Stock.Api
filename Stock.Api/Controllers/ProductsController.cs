using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock.Application.DTOs;
using Stock.Application.Interfaces;
using Stock.Application.Models;

namespace Stock.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductsController : ControllerBase
	{
		private readonly IProductService _service;

		public ProductsController(IProductService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<ActionResult<PagedResult<ProductResponseDto>>> GetAll(
			[FromQuery] string? name,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{
			var result = await _service.GetAllAsync(new ProductFilter
			{
				Name = name,
				Page = page,
				PageSize = pageSize
			});

			return Ok(result);
		}

		[HttpGet("{id:int}")]
		public async Task<ActionResult<ProductResponseDto>> GetById(int id)
		{
			var product = await _service.GetByIdAsync(id);
			if (product is null) return NotFound();
			return Ok(product);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<ActionResult<ProductResponseDto>> Create([FromBody] CreateProductDto dto)
		{
			var created = await _service.CreateAsync(dto);
			return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
		}

		[Authorize(Roles = "Admin")]
		[HttpPut("{id:int}")]
		public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
		{
			var ok = await _service.UpdateAsync(id, dto);
			if (!ok) return NotFound();
			return NoContent();
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{id:int}")]
		public async Task<IActionResult> Delete(int id)
		{
			var ok = await _service.DeleteAsync(id);
			if (!ok) return NotFound();
			return NoContent();
		}
	}
}
