using Stock.Application.Models;

namespace Stock.Application.Interfaces
{
	public interface IProductService
	{
		Task<PagedResult<Product>> GetAllAsync(ProductFilter filter);

		Task<Product?> GetByIdAsync(int id);
		Task<Product> CreateAsync(Product product);
		Task<bool> UpdateAsync(int id, Product product);
		Task<bool> DeleteAsync(int id);
	}
}

