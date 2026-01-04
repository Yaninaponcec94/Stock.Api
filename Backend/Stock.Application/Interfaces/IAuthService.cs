using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Application.Interfaces
{
	public interface IAuthService
	{
		Task<AuthResult?> LoginAsync(string username, string password);
	}
	public class AuthResult
	{
		public string Token { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
	}
}
