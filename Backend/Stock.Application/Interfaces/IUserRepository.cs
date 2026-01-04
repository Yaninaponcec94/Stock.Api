using Stock.Application.Models;

namespace Stock.Application.Interfaces
{
	public interface IUserRepository
	{
		Task<AuthUser?> GetActiveByUsernameAsync(string username);
	}
}
