using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Stock.Api.DTOs;

namespace Stock.Api.Validators
{
	public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
	{
		public UpdateProductDtoValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty()
				.MaximumLength(100);

			RuleFor(x => x.MinStock)
				.GreaterThanOrEqualTo(0);
		}
	}
}

