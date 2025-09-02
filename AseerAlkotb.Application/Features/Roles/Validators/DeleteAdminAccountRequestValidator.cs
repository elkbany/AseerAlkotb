using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Roles.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Roles.Validators
{
    public class DeleteAdminAccountRequestValidator :AbstractValidator<DeleteAdminAccountRequest>
    {
        public DeleteAdminAccountRequestValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty().WithMessage("UserId is required.")
               .GreaterThan(0).WithMessage("UserId must be greater than 0.");
        }
    }
}
