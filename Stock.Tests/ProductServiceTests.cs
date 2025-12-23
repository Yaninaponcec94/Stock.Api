using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Stock.Application.Interfaces;
using Stock.Application.Models;
using Stock.Application.Services;
using Xunit;

namespace Stock.Tests
{
	public class ProductServiceTests
	{
		[Fact]
		public async Task GetAllAsync_CallsRepoGetPagedActiveAsync()
		{
			var repo = new Mock<IProductRepository>();
			var service = new ProductService(repo.Object);

			var filter = new ProductFilter { Name = "teclado", Page = 1, PageSize = 10 };

			repo.Setup(r => r.GetPagedActiveAsync(filter))
				.ReturnsAsync(new PagedResult<Product>
				{
					Items = new List<Product>(),
					Page = 1,
					PageSize = 10,
					
				});

			var result = await service.GetAllAsync(filter);

			Assert.NotNull(result);
			repo.Verify(r => r.GetPagedActiveAsync(filter), Times.Once);
		}
		[Fact]
		public async Task GetByIdAsync_CallsRepoGetActiveByIdAsync()
		{
			var repo = new Mock<IProductRepository>();
			var service = new ProductService(repo.Object);

			repo.Setup(r => r.GetActiveByIdAsync(5))
				.ReturnsAsync(new Product { Id = 5, Name = "Mouse", IsActive = true, MinStock = 0 });

			var product = await service.GetByIdAsync(5);
			Assert.NotNull(product);
			Assert.Equal(5, product!.Id);
			repo.Verify(r => r.GetActiveByIdAsync(5), Times.Once);


		}

		[Fact]
		public async Task CreateAsync_TrimsName_AndCallsAddAsync()
		{
			var repo = new Mock<IProductRepository>();
			var service = new ProductService(repo.Object);

			var p = new Product
			{
				Name = "  Teclado   ",
				IsActive = true,
				MinStock = 2
			};

			await service.CreateAsync(p);

			Assert.Equal("Teclado", p.Name);
			repo.Verify(r => r.AddAsync(It.Is<Product>(x => x.Name == "Teclado")), Times.Once);

		}

		[Fact]
		public async Task UpdateAsync_WhenProductNotFound_ReturnsFalse_AndDoesNotUpdate()
		{
			var repo = new Mock<IProductRepository>();
			var service = new ProductService(repo.Object);

			repo.Setup(r => r.GetActiveByIdAsync(10)).ReturnsAsync((Product?)null);

			var ok = await service.UpdateAsync(10, new Product { Name = "X", IsActive = true, MinStock = 0 });

			Assert.False(ok);
			repo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
		}
		[Fact]
		public async Task UpdateAsync_WhenFound_UpdatesFields_TrimsName_AndCallsUpdateAsync()
		{
			var repo = new Mock<IProductRepository>();
			var service = new ProductService(repo.Object);

			var existing = new Product { Id = 3, Name = "Viejo", IsActive = true, MinStock = 1 };

			repo.Setup(r => r.GetActiveByIdAsync(3)).ReturnsAsync(existing);

			var ok = await service.UpdateAsync(3, new Product { Name = "  Nuevo  ", IsActive = false, MinStock = 9 });

			Assert.True(ok);
			Assert.Equal("Nuevo", existing.Name);
			Assert.False(existing.IsActive);
			Assert.Equal(9, existing.MinStock);
			repo.Verify(r => r.UpdateAsync(existing), Times.Once);

		}

		[Fact]
		public async Task DeleteAsync_CallsRepoSoftDeleteAsync()
		{
			var repo = new Mock<IProductRepository>();
			var service = new ProductService(repo.Object);
			repo.Setup(r => r.SoftDeleteAsync(7)).ReturnsAsync(true);
			var ok = await service.DeleteAsync(7);
			Assert.True(ok);
			repo.Verify(r => r.SoftDeleteAsync(7), Times.Once);
		}
	}

}
