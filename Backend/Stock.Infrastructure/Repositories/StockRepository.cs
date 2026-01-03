using Microsoft.EntityFrameworkCore;
using Stock.Application.Exceptions;
using Stock.Application.Interfaces;
using Stock.Application.Models;
using Stock.Infrastructure.Data;
using Stock.Infrastructure.Entities;


namespace Stock.Infrastructure.Repositories
{
	public class StockRepository : IStockRepository
	{
		private readonly StockDbContext _context;

		public StockRepository(StockDbContext context)
		{
			_context = context;
		}

		public Task<bool> ProductExistsAndActiveAsync(int productId)
			=> _context.Products.AnyAsync(p => p.Id == productId && p.IsActive);

		public async Task<int> GetCurrentQuantityAsync(int productId)
		{
			var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == productId);
			return stock?.Quantity ?? 0;
		}

		public async Task<ProductStockResult> ApplyMovementAsync(int productId, StockMovementType type, int quantity, string? reason)
		{
			var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == productId);
			if (stock == null)
			{
				stock = new ProductStock { ProductId = productId, Quantity = 0, UpdatedAt = DateTime.UtcNow };
				_context.Stocks.Add(stock);
			}

			var newQty = stock.Quantity;

			switch (type)
			{
				case StockMovementType.Entry:
					newQty += quantity;
					break;

				case StockMovementType.Exit:
					if (quantity > stock.Quantity)
						throw new InvalidOperationException("Stock insuficiente para realizar la salida");
					newQty -= quantity;
					break;

				case StockMovementType.Adjustment:
					newQty = quantity;
					break;
			}

			var movement = new StockMovement
			{
				ProductId = productId,
				Type = type,
				Quantity = quantity,
				Reason = reason,
				Date = DateTime.UtcNow
			};

			_context.StockMovements.Add(movement);

			stock.Quantity = newQty;
			stock.UpdatedAt = DateTime.UtcNow;

			try { await _context.SaveChangesAsync(); }
			catch (DbUpdateConcurrencyException)
			{
				throw new ConcurrencyException("El stock fue actualizado por otra operación. Reintente.");
			}

			return new ProductStockResult { ProductId = productId, NewQuantity = stock.Quantity, MovementId = movement.Id };
		}

		public async Task<List<StockItemResult>> GetStockAsync()
		{
			return await _context.Products
				.Where(p => p.IsActive)
				.GroupJoin(
					_context.Stocks,
					p => p.Id,
					s => s.ProductId,
					(p, stocks) => new { p, stock = stocks.FirstOrDefault() }
				)
				.Select(x => new StockItemResult
				{
					ProductId = x.p.Id,
					ProductName = x.p.Name ?? "",
					MinStock = x.p.MinStock,

					Quantity = x.stock != null ? x.stock.Quantity : 0,
					UpdatedAt = x.stock != null ? x.stock.UpdatedAt : DateTime.UtcNow,

					IsBelowMinStock =
						(x.stock != null ? x.stock.Quantity : 0) < x.p.MinStock
				})
				.ToListAsync();
		}

		public async Task<PagedResult<StockMovementHistory>> GetMovementsAsync(int? productId, int page, int pageSize)
		{
			page = page < 1 ? 1 : page;
			pageSize = pageSize < 1 ? 10 : pageSize;
			if (pageSize > 50) pageSize = 50;

			var query = _context.StockMovements
				.AsNoTracking()
				.Include(m => m.Product)
				.AsQueryable();

			if (productId.HasValue)
				query = query.Where(m => m.ProductId == productId.Value);

			var total = await query.CountAsync();

			var items = await query
				.OrderByDescending(m => m.Date)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(m => new StockMovementHistory
				{
					Id = m.Id,
					ProductId = m.ProductId,
					ProductName = m.Product.Name,
					Type = m.Type.ToString(),
					Quantity = m.Quantity,
					Reason = m.Reason,
					Date = m.Date
				})
				.ToListAsync();

			return new PagedResult<StockMovementHistory>
			{
				Items = items,
				TotalItems = total,
				Page = page,
				PageSize = pageSize
			};

		}


	}

}

