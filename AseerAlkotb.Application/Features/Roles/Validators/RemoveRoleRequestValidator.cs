using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Roles.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Roles.Validators
{
   public class RemoveRoleRequestValidator :AbstractValidator<RemoveRoleRequest>
    {
        public RemoveRoleRequestValidator() {
            RuleFor(x => x.UserId)
               .NotEmpty().WithMessage("UserId is required.")
               .GreaterThan(0).WithMessage("UserId must be greater than 0.");

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Invalid role specified. Valid roles are: Client, Staff, Admin");
        }
    }
}
