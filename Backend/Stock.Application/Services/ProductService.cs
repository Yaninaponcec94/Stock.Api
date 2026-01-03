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

		public Task<PagedResult<Product>> GetAllAsync(ProductFilter filter)
			=> _repo.GetPagedActiveAsync(filter);

		public Task<Product?> GetByIdAsync(int id)
			=> _repo.GetActiveByIdAsync(id);

		public async Task<Product> CreateAsync(string name, int minStock)
		{
			var entity = new Product
			{
				Name = name.Trim(),
				IsActive = true,
				MinStock = minStock
			};

			await _repo.AddAsync(entity);
			return entity;
		}

		public async Task<bool> UpdateAsync(int id, string name, bool isActive, int minStock)
		{
			var existing = await _repo.GetActiveByIdAsync(id);
			if (existing == null) return false;

			existing.Name = name.Trim();
			existing.IsActive = isActive;
			existing.MinStock = minStock;

			await _repo.UpdateAsync(existing);
			return true;
		}

		public Task<bool> DeleteAsync(int id)
			=> _repo.SoftDeleteAsync(id);
	}
}
