using Microsoft.AspNetCore.Mvc;
using Stock.Api.DTOs;
using Stock.Application.Interfaces;

namespace Stock.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _auth;

		public AuthController(IAuthService auth)
		{
			_auth = auth;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto dto)
		{
			var result = await _auth.LoginAsync(dto.Username, dto.Password);

			if (result == null)
				return Unauthorized(new { message = "Credenciales inválidas" });

			return Ok(new
			{
				token = result.Token,
				role = result.Role,
				username = result.Username
			});
		}
	}
}
