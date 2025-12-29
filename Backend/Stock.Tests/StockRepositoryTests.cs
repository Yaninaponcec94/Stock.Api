using Microsoft.EntityFrameworkCore;
using Stock.Infrastructure.Data;
using Stock.Infrastructure.Entities;
using Stock.Infrastructure.Repositories;
using Xunit;

namespace Stock.Tests
{
	public class StockRepositoryTests
	{
		private static StockDbContext CreateDb(string dbName)
		{
			var options = new DbContextOptionsBuilder<StockDbContext>()
				.UseInMemoryDatabase(dbName)
				.Options;

			return new StockDbContext(options);
		}

		[Fact]
		public async Task GetStockAsync_ReturnsAllStocks()
		{
			using var db = CreateDb(nameof(GetStockAsync_ReturnsAllStocks));

			db.Products.AddRange(
				new Product { Id = 3, Name = "Prod 3", IsActive = true, MinStock = 0 },
				new Product { Id = 6, Name = "Prod 6", IsActive = true, MinStock = 0 }
			);

			db.Stocks.AddRange(
				new ProductStock { ProductId = 3, Quantity = 12, UpdatedAt = DateTime.UtcNow, RowVersion = new byte[] { 1 } },
				new ProductStock { ProductId = 6, Quantity = 2, UpdatedAt = DateTime.UtcNow, RowVersion = new byte[] { 1 } }
				);


			await db.SaveChangesAsync();

			var repo = new StockRepository(db);
			var result = await repo.GetStockAsync();

			Assert.Equal(2, result.Count);
			Assert.Contains(result, x => x.ProductId == 3 && x.Quantity == 12);
			Assert.Contains(result, x => x.ProductId == 6 && x.Quantity == 2);
		}
	}
}
