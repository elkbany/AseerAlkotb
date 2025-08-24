using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Publishers.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Publishers.Validators
{
    public class GetFollowedPublisherRequestValidation:AbstractValidator<GetFollowedPublisherRequest>
    {
        public GetFollowedPublisherRequestValidation()
        {
            RuleFor(x => x.UserId)
              .NotEmpty()
              .GreaterThan(0)
              .WithMessage("User ID must be greater than 0");

        }
    }
}
