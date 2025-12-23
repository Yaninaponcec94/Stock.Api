using Stock.Application.DTOs;
using Stock.Application.Models;

namespace Stock.Application.Interfaces
{
	public interface IProductService
	{
		Task<PagedResult<ProductResponseDto>> GetAllAsync(ProductFilter filter);
		Task<ProductResponseDto?> GetByIdAsync(int id);

		Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
		Task<bool> UpdateAsync(int id, UpdateProductDto dto);
		Task<bool> DeleteAsync(int id);
	}
}
