using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Publishers.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Authors.Validators
{
   public class IsFollowingAuthorRequestValidator : AbstractValidator<IsFollowingAuthorRequest>
    {
        public IsFollowingAuthorRequestValidator() 
        {
            RuleFor(x => x.authorId)
             .NotEmpty()
             .GreaterThan(0)
            .WithMessage("Publisher ID must be greater than 0");
        }
       
   }


}
