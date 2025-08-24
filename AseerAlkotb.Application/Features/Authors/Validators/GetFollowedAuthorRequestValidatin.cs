using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Authors.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Authors.Validators
{
    public class GetFollowedAuthorRequestValidatin : AbstractValidator<GetFollowedAuthorRequest>
    {
        public GetFollowedAuthorRequestValidatin() {
            RuleFor(x => x.UserId)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");
        }
    }
  
}
