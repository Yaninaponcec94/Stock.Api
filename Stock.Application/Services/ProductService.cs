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
		public async Task<bool> DeleteAsync(int id)
			=> await _repo.SoftDeleteAsync(id);

		public async Task<Product> CreateAsync(Product product)
		{
			product.Name = product.Name.Trim();

			await _repo.AddAsync(product);
			return product;
		}

		public async Task<bool> UpdateAsync(int id, Product product)
		{
			var existing = await _repo.GetActiveByIdAsync(id);
			if (existing == null) return false;

			existing.Name = product.Name.Trim();
			existing.IsActive = product.IsActive;
			existing.MinStock = product.MinStock;

			await _repo.UpdateAsync(existing);
			return true;
		}

	}
}
