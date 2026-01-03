
using Stock.Application.Models;

namespace Stock.Application.Interfaces
{
	public interface IProductService
	{
		Task<PagedResult<Product>> GetAllAsync(ProductFilter filter);
		Task<Product?> GetByIdAsync(int id);

		Task<Product> CreateAsync(string name, int minStock);
		Task<bool> UpdateAsync(int id, string name, bool isActive, int minStock);
		Task<bool> DeleteAsync(int id);
	}
}
