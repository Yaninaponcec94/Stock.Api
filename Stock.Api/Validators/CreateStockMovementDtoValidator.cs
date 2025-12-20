using FluentValidation;
using Stock.Api.DTOs;

namespace Stock.Api.Validators
{
	public class CreateStockMovementDtoValidator : AbstractValidator<CreateStockMovementDto>
	{
		public CreateStockMovementDtoValidator()
		{
			RuleFor(x => x.ProductId)
				.GreaterThan(0);

			RuleFor(x => x.Quantity)
				.GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0");

			RuleFor(x => x.Type)
				.NotEmpty()
				.Must(t => t == "Entry" || t == "Exit" || t == "Adjustment")
				.WithMessage("Type debe ser: Entry, Exit o Adjustment");

			RuleFor(x => x.Reason)
				.MaximumLength(250);
		}
	}
}
