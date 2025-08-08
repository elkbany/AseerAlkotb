using AseerAlkotb.Application.Features.Categories.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Categories.Validators
{
    public class GetAllCategoriesPaginatedRequestValidator : AbstractValidator<GetAllCategoriesPaginatedRequest>
    {
        public GetAllCategoriesPaginatedRequestValidator()
        {
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
                .When(x => !string.IsNullOrEmpty(x.Search))
                .WithMessage("Search term contains invalid characters");
        }

        private bool BeValidSearchTerm(string search)
        {
            if (string.IsNullOrEmpty(search)) return true;

            return System.Text.RegularExpressions.Regex.IsMatch(search, @"^[a-zA-Z\u0600-\u06FF0-9\s\-_.]+$");
        }
    }
}