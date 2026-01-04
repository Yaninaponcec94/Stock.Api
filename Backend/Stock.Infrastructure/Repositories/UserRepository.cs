using Microsoft.EntityFrameworkCore;
using Stock.Application.Interfaces;
using Stock.Application.Models;
using Stock.Infrastructure.Data;

namespace Stock.Infrastructure.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly StockDbContext _context;

		public UserRepository(StockDbContext context)
		{
			_context = context;
		}

		public async Task<AuthUser?> GetActiveByUsernameAsync(string username)
		{
			return await _context.Users
				.AsNoTracking()
				.Where(u => u.Username == username && u.IsActive)
				.Select(u => new AuthUser
				{
					Id = u.Id,
					Username = u.Username,
					PasswordHash = u.PasswordHash,
					Role = u.Role.ToString()
				})
				.FirstOrDefaultAsync();
		}
	}
}
