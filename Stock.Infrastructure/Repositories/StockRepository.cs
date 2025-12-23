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

		public async Task<ProductStockResult> ApplyMovementAsync(int productId, string type, int quantity, string? reason)
		{
			if (!Enum.TryParse<MovementType>(type, out var movementType))
				throw new InvalidOperationException("Tipo de movimiento inválido");

			var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == productId);
			if (stock == null)
			{
				stock = new ProductStock
				{
					ProductId = productId,
					Quantity = 0,
					UpdatedAt = DateTime.UtcNow
				};
				_context.Stocks.Add(stock);
			}

			var newQty = stock.Quantity;

			switch (movementType)
			{
				case MovementType.Entry:
					newQty += quantity;
					break;

				case MovementType.Exit:

					if (quantity > stock.Quantity)
						throw new InvalidOperationException("Stock insuficiente para realizar la salida");
					newQty -= quantity;
					break;

				case MovementType.Adjustment:
					newQty = quantity;
					break;
			}

			var movement = new StockMovement
			{
				ProductId = productId,
				Type = movementType,
				Quantity = quantity,
				Reason = reason,
				Date = DateTime.UtcNow
			};

			_context.StockMovements.Add(movement);

			stock.Quantity = newQty;
			stock.UpdatedAt = DateTime.UtcNow;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				throw new ConcurrencyException("El stock fue actualizado por otra operación. Reintente.");
			}

			return new ProductStockResult
			{
				ProductId = productId,
				NewQuantity = stock.Quantity,
				MovementId = movement.Id
			};
		}
		public async Task<List<StockItemResult>> GetStockAsync()
		{
			return await _context.Stocks
				.Join(_context.Products,
					s => s.ProductId,
					p => p.Id,
					(s, p) => new StockItemResult
					{
						ProductId = s.ProductId,
						Quantity = s.Quantity,
						UpdatedAt = s.UpdatedAt,
						MinStock = p.MinStock,
						IsBelowMinStock = s.Quantity <= p.MinStock
					})
				.ToListAsync();
		}


	}

}

