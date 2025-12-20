using Microsoft.AspNetCore.Mvc;
using Stock.Api.DTOs;
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
		public async Task<ActionResult<PagedResult<Product>>> GetAll([FromQuery] string? name, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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
		public async Task<ActionResult<Product>> GetById(int id)
		{
			var product = await _service.GetByIdAsync(id);
			if (product is null) return NotFound();
			return Ok(product);
		}

		[HttpPost]
		public async Task<ActionResult<Product>> Create(CreateProductDto dto)
		{
			var created = await _service.CreateAsync(new Product
			{
				Name = dto.Name,
				IsActive = true,
				MinStock = dto.MinStock
			});

			return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
		}

		[HttpPut("{id:int}")]
		public async Task<IActionResult> Update(int id, UpdateProductDto dto)
		{
			var ok = await _service.UpdateAsync(id, new Product
			{
				Name = dto.Name,
				IsActive = dto.IsActive,
				MinStock = dto.MinStock
			});

			if (!ok) return NotFound();
			return NoContent();
		}

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> Delete(int id)
		{
			var ok = await _service.DeleteAsync(id);
			if (!ok) return NotFound();
			return NoContent();
		}
	}
}
