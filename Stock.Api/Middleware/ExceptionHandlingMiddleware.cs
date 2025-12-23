using System.Net;
using System.Text.Json;

namespace Stock.Api.Middleware
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (InvalidOperationException ex)
			{
				await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
			}
			catch (Exception ex)
			{
				await WriteError(context, HttpStatusCode.InternalServerError, "Error inesperado");
			}
		}

		private static async Task WriteError(HttpContext context, HttpStatusCode status, string message)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)status;

			var payload = JsonSerializer.Serialize(new { message });
			await context.Response.WriteAsync(payload);
		}
	}
}
