using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Categories.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Categories.Validators
{
    public class GetAllSubCategoriesPaginatedRequestValidator : AbstractValidator<GetAllSubCategoriesPaginatedRequest>
    {
        public GetAllSubCategoriesPaginatedRequestValidator()
        {
            RuleFor(x => x.ParentCategoryId)
                .GreaterThan(0)
                .WithMessage("ParentCategoryId must be greater than 0");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Page size must be greater than 0")
                .LessThanOrEqualTo(100)
                .WithMessage("Page size cannot exceed 100 records");

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .WithMessage("Search term cannot exceed 100 characters")
                .Must(BeValidSearchTerm)
                .When(x => !string.IsNullOrWhiteSpace(x.Search))
                .WithMessage("Search term contains invalid characters");
        }

        private bool BeValidSearchTerm(string search)
        {
            return Regex.IsMatch(search, @"^[a-zA-Z\u0600-\u06FF0-9\s\-_.]+$");
        }
    }
}