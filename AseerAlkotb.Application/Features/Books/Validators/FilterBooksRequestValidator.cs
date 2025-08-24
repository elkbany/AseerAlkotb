using AseerAlkotb.Application.Features.Books.Requests;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Books.Validators
{
    public class FilterBooksRequestValidator : AbstractValidator<FilterBooksRequest>
    {
        public FilterBooksRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .L("PageNumber", "MustBeGreaterThan" , "0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .L("PageSize", "MustBeBetween", "1", "100");

            RuleFor(x => x.SortBy)
                .IsInEnum()
                .When(x => x.SortBy.HasValue)
                .L("SortOption", "Invalid");
        }
    }
}
