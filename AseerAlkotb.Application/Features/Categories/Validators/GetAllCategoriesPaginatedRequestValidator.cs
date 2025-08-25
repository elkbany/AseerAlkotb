using AseerAlkotb.Application.Features.Categories.Requests;
using FluentValidation;
using AseerAlkotb.Application.ResponseHandler; // عشان الـ Extension L
using AseerAlkotb.Localization.Resources;
using Microsoft.Extensions.Localization;

namespace AseerAlkotb.Application.Features.Categories.Validators
{
    public class GetAllCategoriesPaginatedRequestValidator : AbstractValidator<GetAllCategoriesPaginatedRequest>
    {
        public GetAllCategoriesPaginatedRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .L("PageNumber", "MustBeGreaterThan" , "0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .L("PageSize", "MustBeGreaterThan" , "0")
                .LessThanOrEqualTo(100)
                .L("PageSize", "CannotExceed", "100");

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .L("Search", "MaxLength", "100")
                .Must(BeValidSearchTerm)
                .When(x => !string.IsNullOrEmpty(x.Search))
                .L("Search", "Invalid" , "Characters");
        }

        private bool BeValidSearchTerm(string search)
        {
            if (string.IsNullOrEmpty(search)) return true;

            return System.Text.RegularExpressions.Regex.IsMatch(search, @"^[a-zA-Z\u0600-\u06FF0-9\s\-_.]+$");
        }
    }
}
