using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Publishers.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Publishers.Validators
{
    public class FollowPublisherRequestValidation : AbstractValidator<FollowPublisherRequest>
    {
        public FollowPublisherRequestValidation() {
            
            RuleFor(x => x.UserId)
               .NotEmpty()
               .GreaterThan(0)
               .WithMessage("User ID must be greater than 0");

            RuleFor(x => x.PublisherId)
              .NotEmpty()
              .GreaterThan(0)
             .WithMessage("Publisher ID must be greater than 0");
        }
    }
    
}
