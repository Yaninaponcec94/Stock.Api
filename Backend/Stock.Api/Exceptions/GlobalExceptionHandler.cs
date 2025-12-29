using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Stock.Application.Exceptions;

namespace Stock.Api.Exceptions
{
	public class GlobalExceptionHandler : IExceptionHandler
	{
		public async ValueTask<bool> TryHandleAsync(
			HttpContext httpContext,
			Exception exception,
			CancellationToken cancellationToken)
		{
			var (statusCode, title) = exception switch
			{
				ConcurrencyException =>
					(StatusCodes.Status409Conflict, "Conflicto de concurrencia"),

				InvalidOperationException =>
					(StatusCodes.Status400BadRequest, "Operación no válida"),

				KeyNotFoundException =>
					(StatusCodes.Status404NotFound, "Recurso no encontrado"),

				_ =>
					(StatusCodes.Status500InternalServerError, "Error interno del servidor")
			};


			var problem = new ProblemDetails
			{
				Status = statusCode,
				Title = title,
				Detail = exception.Message
			};

			httpContext.Response.StatusCode = statusCode;
			httpContext.Response.ContentType = "application/problem+json";

			await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
			return true;
		}
	}
}
