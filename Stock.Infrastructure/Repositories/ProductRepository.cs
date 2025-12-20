using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stock.Application.Interfaces;
using Stock.Application.Models;
using Stock.Infrastructure.Data;

namespace Stock.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly StockDbContext _context;
        public ProductRepository(StockDbContext context)
        {
            _context = context;
		}

		public async Task<PagedResult<Product>> GetPagedActiveAsync(ProductFilter filter)
		{
			var page = filter.Page < 1 ? 1 : filter.Page;
			var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;
			if (pageSize > 50) pageSize = 50; 

			var query = _context.Products.AsNoTracking()
				.Where(p => p.IsActive);

		
			if (!string.IsNullOrWhiteSpace(filter.Name))
			{
				var name = filter.Name.Trim();
				query = query.Where(p => p.Name != null && p.Name.Contains(name));
			}

			var totalItems = await query.CountAsync();

			var items = await query
				.OrderBy(p => p.Id)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(p => new Stock.Application.Models.Product
				{
					Id = p.Id,
					Name = p.Name,
					IsActive = p.IsActive,
					MinStock = p.MinStock
				})
				.ToListAsync();

			return new PagedResult<Product>
			{
				Items = items,
				TotalItems = totalItems,
				Page = page,
				PageSize = pageSize
			};
		}


		public async Task<Product?> GetActiveByIdAsync(int id)
		{
			return await _context.Products
				.Where(p => p.Id == id)
				.Select(p => new Product
				{
					Id = p.Id,
					Name = p.Name,
					IsActive = p.IsActive,
					MinStock = p.MinStock
				})
				.FirstOrDefaultAsync();
		}

		public async Task AddAsync(Product product)
		{
			var entity = new Stock.Infrastructure.Entities.Product
			{
				Name = product.Name,
				IsActive = product.IsActive,
				MinStock = product.MinStock
			};


			_context.Products.Add(entity);
			await _context.SaveChangesAsync();

			product.Id = entity.Id;
		}

		public async Task UpdateAsync(Product product)
		{
			var entity = await _context.Products.FindAsync(product.Id);

			if (entity == null)
				return;

			entity.Name = product.Name;
			entity.IsActive = product.IsActive;
			entity.MinStock = product.MinStock;

			await _context.SaveChangesAsync();
		}
		public async Task <bool> SoftDeleteAsync(int id)
		{ 
			var entity = await _context.Products.FindAsync(id);

			if (entity == null)
				return false;
			if (!entity.IsActive)
				return true;
			entity.IsActive = false;
			await _context.SaveChangesAsync();
			return true;
		}

	}

}
