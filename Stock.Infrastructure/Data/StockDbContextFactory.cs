using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Stock.Infrastructure.Data
{
	public class StockDbContextFactory
		: IDesignTimeDbContextFactory<StockDbContext>
	{
		public StockDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<StockDbContext>();

			optionsBuilder.UseSqlServer(
				"Server=localhost\\SQLEXPRESS;Database=StockManagementDb;Trusted_Connection=True;TrustServerCertificate=True");

			return new StockDbContext(optionsBuilder.Options);
		}
	}
}

