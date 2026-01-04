using Stock.Api.Validators;
using Xunit;
using Stock.Api.DTOs;

namespace Stock.Tests
{
	public class ProductValidatorsTests
	{
		[Fact]
		public void CreateProductDto_WhenNameEmpty_ShouldFail()
		{
			var validator = new CreateProductDtoValidator();
			var dto = new CreateProductDto { Name = "", MinStock = 0 };

			var result = validator.Validate(dto);

			Assert.False(result.IsValid);
			Assert.Contains(result.Errors, e => e.PropertyName == "Name");
		}

		[Fact]
		public void CreateProductDto_WhenMinStockNegative_ShouldFail()
		{
			var validator = new CreateProductDtoValidator();
			var dto = new CreateProductDto { Name = "Teclado", MinStock = -1 };

			var result = validator.Validate(dto);

			Assert.False(result.IsValid);
			Assert.Contains(result.Errors, e => e.PropertyName == "MinStock");
		}

		[Fact]
		public void CreateProductDto_WhenValid_ShouldPass()
		{
			var validator = new CreateProductDtoValidator();
			var dto = new CreateProductDto { Name = "Teclado", MinStock = 0 };

			var result = validator.Validate(dto);

			Assert.True(result.IsValid);
		}

		[Fact]
		public void UpdateProductDto_WhenNameEmpty_ShouldFail()
		{
			var validator = new UpdateProductDtoValidator();
			var dto = new UpdateProductDto { Name = "", MinStock = 0, IsActive = true };

			var result = validator.Validate(dto);

			Assert.False(result.IsValid);
			Assert.Contains(result.Errors, e => e.PropertyName == "Name");
		}

		[Fact]
		public void UpdateProductDto_WhenMinStockNegative_ShouldFail()
		{
			var validator = new UpdateProductDtoValidator();
			var dto = new UpdateProductDto { Name = "Mouse", MinStock = -5, IsActive = true };

			var result = validator.Validate(dto);

			Assert.False(result.IsValid);
			Assert.Contains(result.Errors, e => e.PropertyName == "MinStock");
		}

		[Fact]
		public void UpdateProductDto_WhenValid_ShouldPass()
		{
			var validator = new UpdateProductDtoValidator();
			var dto = new UpdateProductDto { Name = "Mouse", MinStock = 2, IsActive = true };

			var result = validator.Validate(dto);

			Assert.True(result.IsValid);
		}
	}
}
