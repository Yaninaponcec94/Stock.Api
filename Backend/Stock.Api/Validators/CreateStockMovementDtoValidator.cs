using FluentValidation;
using Stock.Api.DTOs;

namespace Stock.Api.Validators
{
	public class CreateStockMovementRequestDtoValidator
		: AbstractValidator<CreateStockMovementRequestDto>
	{
		public CreateStockMovementRequestDtoValidator()
		{
			RuleFor(x => x.ProductId)
				.GreaterThan(0)
				.WithMessage("Producto inválido.");

			RuleFor(x => x.Quantity)
				.GreaterThan(0)
				.WithMessage("La cantidad debe ser mayor a 0.");

			RuleFor(x => x.Reason)
				.MaximumLength(250)
				.WithMessage("El motivo no puede superar 250 caracteres.");
		}
	}
}
