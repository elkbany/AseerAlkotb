using AseerAlkotb.Application.Features.Publishers.Requests;
using FluentValidation;
using AseerAlkotb.Application.ResponseHandler; // للـ Extension L
using AseerAlkotb.Domain.Resources;
using Microsoft.Extensions.Localization;

namespace AseerAlkotb.Application.Features.Publishers.Validators
{
    public class DeletePublisherRequestValidator : AbstractValidator<DeletePublisherRequest>
    {
        public DeletePublisherRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .L("Publisher", "Id", "MustBeGreaterThan", "0");
        }
    }
}
