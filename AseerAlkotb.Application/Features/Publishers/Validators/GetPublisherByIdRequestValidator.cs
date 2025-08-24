using AseerAlkotb.Application.Features.Publishers.Requests;
using FluentValidation;
using AseerAlkotb.Application.ResponseHandler; // للـ Extension L
using AseerAlkotb.Domain.Resources;
using Microsoft.Extensions.Localization;

namespace AseerAlkotb.Application.Features.Publishers.Validators
{
    public class GetPublisherByIdRequestValidator : AbstractValidator<GetPublisherByIdRequest>
    {
        public GetPublisherByIdRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .GreaterThan(0)
                .L("Publisher", "Id" ,"MustBeGreaterThan" , "0");
        }
    }
}
