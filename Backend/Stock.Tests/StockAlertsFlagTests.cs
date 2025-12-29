using Microsoft.EntityFrameworkCore;
using Stock.Infrastructure.Data;
using Stock.Infrastructure.Entities;
using Stock.Infrastructure.Repositories;
using Xunit;

namespace Stock.Tests
{
	public class StockAlertsFlagTests
	{
		private static StockDbContext CreateDb(string dbName)
		{
			var options = new DbContextOptionsBuilder<StockDbContext>()
				.UseInMemoryDatabase(dbName)
				.Options;

			return new StockDbContext(options);
		}

		[Fact]
		public async Task GetStockAsync_SetsIsBelowMinStock_WhenQuantityIsLessOrEqualThanMinStock()
		{
			using var db = CreateDb(nameof(GetStockAsync_SetsIsBelowMinStock_WhenQuantityIsLessOrEqualThanMinStock));

			db.Products.AddRange(
				new Product { Id = 1, Name = "Prod 1", IsActive = true, MinStock = 5 },
				new Product { Id = 2, Name = "Prod 2", IsActive = true, MinStock = 2 }
			);

			db.Stocks.AddRange(
				new ProductStock { ProductId = 1, Quantity = 6, UpdatedAt = DateTime.UtcNow, RowVersion = new byte[] { 1 } }, // > min => false
				new ProductStock { ProductId = 2, Quantity = 2, UpdatedAt = DateTime.UtcNow, RowVersion = new byte[] { 1 } }  // == min => true
			);

			await db.SaveChangesAsync();

			var repo = new StockRepository(db);

			var result = await repo.GetStockAsync();

			var p1 = Assert.Single(result, x => x.ProductId == 1);
			Assert.False(p1.IsBelowMinStock);

			var p2 = Assert.Single(result, x => x.ProductId == 2);
			Assert.True(p2.IsBelowMinStock);
		}
	}
}
