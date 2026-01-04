using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Stock.Application.Interfaces;

namespace Stock.Application.Services
{
	public class AuthService : IAuthService
	{
		private readonly IUserRepository _users;
		private readonly IConfiguration _config;

		public AuthService(IUserRepository users, IConfiguration config)
		{
			_users = users;
			_config = config;
		}

		public async Task<AuthResult?> LoginAsync(string username, string password)
		{
			var user = await _users.GetActiveByUsernameAsync(username);

			if (user == null)
				return null;

			var ok = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
			if (!ok)
				return null;

			var jwt = _config.GetSection("Jwt");
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.Username),
				new Claim(ClaimTypes.Role, user.Role.ToString())
			};

			var token = new JwtSecurityToken(
				issuer: jwt["Issuer"],
				audience: jwt["Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiresMinutes"]!)),
				signingCredentials: creds
			);

			var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

			return new AuthResult
			{
				Token = tokenString,
				Role = user.Role.ToString(),
				Username = user.Username
			};
		}
	}
}
