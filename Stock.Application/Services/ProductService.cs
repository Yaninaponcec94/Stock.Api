using Stock.Application.DTOs;
using Stock.Application.Interfaces;
using Stock.Application.Models;

namespace Stock.Application.Services
{
	public class ProductService : IProductService
	{
		private readonly IProductRepository _repo;

		public ProductService(IProductRepository repo)
		{
			_repo = repo;
		}

		public async Task<PagedResult<ProductResponseDto>> GetAllAsync(ProductFilter filter)
		{
			var paged = await _repo.GetPagedActiveAsync(filter);

			// Mapeo en service (no en controller)
			var mapped = new PagedResult<ProductResponseDto>
			{
				Page = paged.Page,
				PageSize = paged.PageSize,
				TotalItems = paged.TotalItems,
				Items = paged.Items.Select(ToResponse).ToList()
			};

			return mapped;
		}

		public async Task<ProductResponseDto?> GetByIdAsync(int id)
		{
			var product = await _repo.GetActiveByIdAsync(id);
			return product is null ? null : ToResponse(product);
		}

		public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
		{
			var entity = new Product
			{
				Name = dto.Name.Trim(),
				IsActive = true,
				MinStock = dto.MinStock
			};

			await _repo.AddAsync(entity);
			return ToResponse(entity);
		}

		public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
		{
			var existing = await _repo.GetActiveByIdAsync(id);
			if (existing == null) return false;

			existing.Name = dto.Name.Trim();
			existing.IsActive = dto.IsActive;
			existing.MinStock = dto.MinStock;

			await _repo.UpdateAsync(existing);
			return true;
		}

		public async Task<bool> DeleteAsync(int id)
			=> await _repo.SoftDeleteAsync(id);

		private static ProductResponseDto ToResponse(Product p) => new()
		{
			Id = p.Id,
			Name = p.Name,
			MinStock = p.MinStock,
			IsActive = p.IsActive
		};
	}
}
