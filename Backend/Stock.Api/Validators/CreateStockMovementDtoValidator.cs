using FluentValidation;
using Stock.Application.DTOs;
using Stock.Application.Models;

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

			When(x => IsAdjustment(x.Type), () =>
			{
				RuleFor(x => x.Reason)
					.NotEmpty().WithMessage("El motivo es obligatorio para un ajuste.")
					.MaximumLength(250).WithMessage("El motivo no puede superar 250 caracteres.");
			}
			);
		}
		private static bool IsAdjustment(string? type)
		{
			if (string.IsNullOrWhiteSpace(type)) return false;
			return type.Trim().Equals(StockMovementType.Adjustment.ToString(), StringComparison.OrdinalIgnoreCase)
				   || type.Trim().Equals("Adjustment", StringComparison.OrdinalIgnoreCase);
		}
	}
	}

