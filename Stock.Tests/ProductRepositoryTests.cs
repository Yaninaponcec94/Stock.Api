using Microsoft.EntityFrameworkCore;
using Stock.Application.Models; // ✅ para ProductFilter y PagedResult
using Stock.Infrastructure.Data;
using Stock.Infrastructure.Repositories;
using Xunit;
using EntityProduct = Stock.Infrastructure.Entities.Product; // ✅ alias para evitar ambigüedad

namespace Stock.Tests
{
	public class ProductRepositoryTests
	{
		private static StockDbContext CreateDb(string dbName)
		{
			var options = new DbContextOptionsBuilder<StockDbContext>()
				.UseInMemoryDatabase(dbName)
				.Options;

			return new StockDbContext(options);
		}

		[Fact]
		public async Task GetPagedActiveAsync_ReturnsOnlyActive_AndAppliesNameFilter()
		{
			using var db = CreateDb(nameof(GetPagedActiveAsync_ReturnsOnlyActive_AndAppliesNameFilter));

			db.Products.AddRange(
				new EntityProduct { Id = 1, Name = "teclado", IsActive = true, MinStock = 0 },
				new EntityProduct { Id = 2, Name = "mouse", IsActive = true, MinStock = 0 },
				new EntityProduct { Id = 3, Name = "teclado gamer", IsActive = false, MinStock = 0 }
				);


			await db.SaveChangesAsync();

			var repo = new ProductRepository(db);

			var filter = new ProductFilter { Name = "teclado", Page = 1, PageSize = 10 };
			var result = await repo.GetPagedActiveAsync(filter);

			Assert.NotNull(result);
			Assert.All(result.Items, p => Assert.True(p.IsActive));
			Assert.All(result.Items, p => Assert.Contains("Teclado", p.Name, StringComparison.OrdinalIgnoreCase));

			Assert.Equal(1, result.Items.Count); 
		}
	}
}
