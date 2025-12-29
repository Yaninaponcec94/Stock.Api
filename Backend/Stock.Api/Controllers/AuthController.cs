using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stock.Application.DTOs;
using Stock.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Stock.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly StockDbContext _db;
		private readonly IConfiguration _config;

		public AuthController(StockDbContext db, IConfiguration config)
		{
			_db = db;
			_config = config;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto dto)
		{
			var user = await _db.Users.FirstOrDefaultAsync(u =>
				u.Username == dto.Username && u.IsActive);

			if (user == null)
				return Unauthorized(new { message = "Credenciales inválidas" });

			var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
			if (!ok)
				return Unauthorized(new { message = "Credenciales inválidas" });

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

			return Ok(new
			{
				token = tokenString,
				role = user.Role.ToString(),
				username = user.Username
			});
		}
	}
}
