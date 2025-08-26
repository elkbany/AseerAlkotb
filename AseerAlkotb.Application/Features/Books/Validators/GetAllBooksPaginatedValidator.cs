using AseerAlkotb.Application.Features.Books.Requests;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;
using System.Text.RegularExpressions;

namespace AseerAlkotb.Application.Features.Books.Validators
{
    public class GetAllBooksPaginatedValidator : AbstractValidator<GetAllBooksPaginatedRequest>
    {
        public GetAllBooksPaginatedValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .L("PageNumber", "MustBeGreaterThan", "0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .L("PageSize", "MustBeGreaterThan", "0")
                .LessThanOrEqualTo(100)
                .L("PageSize", "CannotExceed", "100", "Records");

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .L("SearchTerm", "CannotExceed", "100", "Characters")
                .Must(BeValidSearchTerm)
                .When(x => !string.IsNullOrEmpty(x.Search))
                .L("SearchTerm", "ContainsInvalidCharacters");
        }

        private bool BeValidSearchTerm(string search)
        {
            if (string.IsNullOrEmpty(search)) return true;
            return Regex.IsMatch(search, @"^[a-zA-Z\u0600-\u06FF0-9\s\-_.]+$");
        }
    }
}
