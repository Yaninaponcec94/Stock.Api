using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock.Infrastructure.Entities;

namespace Stock.Infrastructure.Configurations
{
	public class StockConfiguration : IEntityTypeConfiguration<ProductStock>
	{
		public void Configure(EntityTypeBuilder<ProductStock> builder)
		{
			builder.ToTable("Stocks");

			builder.HasKey(s => s.ProductId);

			builder.Property(s => s.Quantity)
				   .IsRequired();

			builder.Property(s => s.UpdatedAt)
				   .IsRequired();

			builder.HasOne(s => s.Product)
				   .WithOne()
				   .HasForeignKey<ProductStock>(s => s.ProductId)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.HasMany(s => s.Movements)
				   .WithOne(m => m.ProductStock!)
				   .HasForeignKey(m => m.ProductId)
				   .HasPrincipalKey(s => s.ProductId);

			builder.Property(s => s.RowVersion)
				.IsRowVersion();
				   
		}
	}
}

