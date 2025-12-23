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
    public class StockServiceTests
    {
        [Fact]
        public async Task CreateMovementAsync_WhenQuantityIsZero_Throws()
        {
            var repo = new Mock<IStockRepository>();
            var service = new StockService(repo.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateMovementAsync(productId: 1, type: "Entry", quantity: 0, reason: "test"));

            Assert.Equal("La cantidad debe ser mayor a 0", ex.Message);
        }

        [Fact]
        public async Task CreateMovementAsync_WhenProductNotExists_Throw()
        {
            var repo = new Mock<IStockRepository>();
            repo.Setup(r => r.ProductExistsAndActiveAsync(3)).ReturnsAsync(false);

            var service = new StockService(repo.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateMovementAsync(3, "Entry", 5, "test"));

			Assert.Equal("Producto no existe o está inactivo", ex.Message);

		}

		[Fact]
        public async Task CreateMovementAsync_WhenExitAndInsufficientStock_Throws()
        {
            var repo = new Mock<IStockRepository>();
            repo.Setup(r => r.ProductExistsAndActiveAsync(3)).ReturnsAsync(true);
            repo.Setup(r => r.GetCurrentQuantityAsync(3)).ReturnsAsync(2);

            var service = new StockService(repo.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateMovementAsync(3, "Exit", 10, "test"));

			Assert.Equal("Stock insuficiente para realizar la salida", ex.Message);


		}

		[Fact]
        public async Task CreateMovementAsync_WhenValidEntry_CallsApplyMovement()
        {
            var repo = new Mock<IStockRepository>();
            repo.Setup(r => r.ProductExistsAndActiveAsync(1)).ReturnsAsync(true);

            repo.Setup(r => r.ApplyMovementAsync(1, "Entry", 5, "ok"))
                .ReturnsAsync(new ProductStockResult { ProductId = 1, NewQuantity = 5, MovementId = 99 });

            var service = new StockService(repo.Object);

            var result = await service.CreateMovementAsync(1, "Entry", 5, "ok");

            Assert.Equal(1, result.ProductId);
            Assert.Equal(5, result.NewQuantity);

            repo.Verify(r => r.ApplyMovementAsync(1, "Entry", 5, "ok"), Times.Once);

        }
    }
}
