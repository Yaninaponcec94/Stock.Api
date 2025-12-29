using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stock.Application.Models;

namespace Stock.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<PagedResult<Product>> GetPagedActiveAsync(ProductFilter filter);

        Task<Product?> GetActiveByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task <bool> SoftDeleteAsync(int id);

	}
}
