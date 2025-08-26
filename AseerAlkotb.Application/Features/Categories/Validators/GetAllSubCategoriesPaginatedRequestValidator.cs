using AseerAlkotb.Application.Features.Categories.Requests;
using FluentValidation;
using AseerAlkotb.Application.ResponseHandler; // عشان الـ Extension L
using AseerAlkotb.Localization.Resources;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;

namespace AseerAlkotb.Application.Features.Categories.Validators
{
    public class GetAllSubCategoriesPaginatedRequestValidator : AbstractValidator<GetAllSubCategoriesPaginatedRequest>
    {
        public GetAllSubCategoriesPaginatedRequestValidator()
        {
            RuleFor(x => x.ParentCategoryId)
                .GreaterThan(0)
                .L("ParentCategoryId", "MustBeGreaterThanZero");

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
                .When(x => !string.IsNullOrWhiteSpace(x.Search))
                .L("Search", "Invalid" , "Characters");
        }

        private bool BeValidSearchTerm(string search)
        {
            return Regex.IsMatch(search, @"^[a-zA-Z\u0600-\u06FF0-9\s\-_.]+$");
        }
    }
}
