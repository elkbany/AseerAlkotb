using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Authors.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Authors.Validators
{
   public class GetFollowersAuthorRequestValidation:AbstractValidator<GetFollowersAuthorRequest>
    {
        public GetFollowersAuthorRequestValidation() {
            RuleFor(x => x.AuthorId)
             .NotEmpty()
             .GreaterThan(0)
             .WithMessage("Author ID must be greater than 0");
        }
    }
}
