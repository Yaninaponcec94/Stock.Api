using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Stock.Api.DTOs;

namespace Stock.Api.Validators
{
	public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
	{
		public CreateProductDtoValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("El nombre es obligatorio")
				.MaximumLength(100);

			RuleFor(x => x.MinStock)
				.GreaterThanOrEqualTo(0)
				.WithMessage("El stock mínimo no puede ser negativo");
		}
	}
}
