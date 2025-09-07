using FluentValidation;
using AseerAlkotb.Application.Features.Orders.Requests;

namespace AseerAlkotb.Application.Features.Orders.Validators
{
    public class GetAllUserOrdersPaginatedRequestValidator : AbstractValidator<GetAllUserOrdersPaginatedRequest>
    {
        public GetAllUserOrdersPaginatedRequestValidator()
        {
           

            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.");
        }
    }
}
