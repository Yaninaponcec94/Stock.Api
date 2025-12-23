using Microsoft.EntityFrameworkCore;
using Stock.Infrastructure.Data;
using Stock.Infrastructure.Entities;

namespace Stock.Api.Seed
{
	public static class DbSeeder
	{
		public static async Task SeedAsync(StockDbContext db)
		{
			Console.WriteLine($"[SEED] DB: {db.Database.GetDbConnection().Database}");

			// (opcional pero recomendado) asegura que la DB esté al día
			await db.Database.MigrateAsync();

			var adminExists = await db.Users.AnyAsync(u => u.Username == "admin");
			Console.WriteLine($"[SEED] adminExists: {adminExists}");

			if (adminExists) return;

			var admin = new User
			{
				Username = "admin",
				PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
				Role = UserRole.Admin,
				IsActive = true,
				CreatedAt = DateTime.UtcNow
			};

			db.Users.Add(admin);
			var rows = await db.SaveChangesAsync();

			Console.WriteLine($"[SEED] Inserted rows: {rows}");
		}
	}
}

