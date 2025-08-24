using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Resources;
using FluentValidation;
using System.Text.RegularExpressions;

namespace AseerAlkotb.Application.Features.Authors.Validators
{
    public class GetAllAuthorsPaginatedRequestValidator : AbstractValidator<GetAllAuthorsPaginatedRequest>
    {
        public GetAllAuthorsPaginatedRequestValidator()
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

            // Allow letters, numbers, spaces, and common punctuation
            return Regex.IsMatch(search, @"^[a-zA-Z\u0600-\u06FF0-9\s\-_.]+$");
        }
    }
}
