using AseerAlkotb.Application.Features.Publishers.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Publishers.Validators
{
    public class GetAllPublishersPaginatedRequestValidator : AbstractValidator<GetAllPublishersPaginatedRequest>
    {
        public GetAllPublishersPaginatedRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100");

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
