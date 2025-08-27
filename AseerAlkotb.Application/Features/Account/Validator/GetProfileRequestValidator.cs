using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Account.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Account.Validator
{
   public class GetProfileRequestValidator : AbstractValidator<GetProfileRequest>
    {
        public GetProfileRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");
        }
    }
}
