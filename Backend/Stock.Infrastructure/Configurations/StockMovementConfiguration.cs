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
	public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
	{
		public void Configure(EntityTypeBuilder<StockMovement> builder)
		{
			builder.ToTable("StockMovements");

			builder.HasKey(m => m.Id);

			builder.Property(m => m.Type)
				   .IsRequired();

			builder.Property(m => m.Quantity)
				   .IsRequired();

			builder.Property(m => m.Date)
				   .IsRequired();

			builder.Property(m => m.Reason)
				   .HasMaxLength(250);

			builder.HasOne(m => m.Product)
				   .WithMany()
				   .HasForeignKey(m => m.ProductId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasIndex(m => new { m.ProductId, m.Date });
		}
	}
}

