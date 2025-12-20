using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stock.Infrastructure.Entities;
using Stock.Infrastructure.Configurations;

namespace Stock.Infrastructure.Data
{
    public class StockDbContext : DbContext
    {
		public StockDbContext(DbContextOptions<StockDbContext> options)
			: base(options)
		{
		}

		public DbSet<Product> Products => Set<Product>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfiguration(new StockConfiguration());
			modelBuilder.ApplyConfiguration(new StockMovementConfiguration());
			modelBuilder.ApplyConfiguration(new ProductConfiguration());
			modelBuilder.ApplyConfiguration(new UserConfiguration());
		}

		public DbSet<ProductStock> Stocks => Set<ProductStock>();
		public DbSet<StockMovement> StockMovements => Set<StockMovement>();

		public DbSet<User> Users => Set<User>();



	}
}
