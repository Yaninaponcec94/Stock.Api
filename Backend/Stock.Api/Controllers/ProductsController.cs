using Microsoft.AspNetCore.Authorization;
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

			var response = new PagedResult<ProductResponseDto>
			{
				Page = result.Page,
				PageSize = result.PageSize,
				TotalItems = result.TotalItems,
				Items = result.Items.Select(p => new ProductResponseDto
				{
					Id = p.Id,
					Name = p.Name,
					MinStock = p.MinStock,
					IsActive = p.IsActive
				}).ToList()
			};

			return Ok(response);
		}


		[HttpGet("{id:int}")]
		public async Task<ActionResult<ProductResponseDto>> GetById(int id)
		{
			var product = await _service.GetByIdAsync(id);
			if (product is null) return NotFound();

			return Ok(new ProductResponseDto
			{
				Id = product.Id,
				Name = product.Name,
				MinStock = product.MinStock,
				IsActive = product.IsActive
			});
		}


		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<ActionResult<ProductResponseDto>> Create([FromBody] CreateProductDto dto)
		{
			var created = await _service.CreateAsync(dto.Name, dto.MinStock);

			return CreatedAtAction(nameof(GetById), new { id = created.Id },
				new ProductResponseDto
				{
					Id = created.Id,
					Name = created.Name,
					MinStock = created.MinStock,
					IsActive = created.IsActive
				});
		}

		[Authorize(Roles = "Admin")]
		[HttpPut("{id:int}")]
		public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
		{
			var ok = await _service.UpdateAsync(id, dto.Name, dto.IsActive, dto.MinStock);
			if (!ok) return NotFound();
			return NoContent();
		}

	}
}
