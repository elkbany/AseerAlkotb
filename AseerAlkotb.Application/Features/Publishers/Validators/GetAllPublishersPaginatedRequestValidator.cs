using AseerAlkotb.Application.Features.Publishers.Requests;
using FluentValidation;
using AseerAlkotb.Application.ResponseHandler; // للـ Extension L
using AseerAlkotb.Domain.Resources;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;

namespace AseerAlkotb.Application.Features.Publishers.Validators
{
    public class GetAllPublishersPaginatedRequestValidator : AbstractValidator<GetAllPublishersPaginatedRequest>
    {
        public GetAllPublishersPaginatedRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .L("PageNumber", "MustBeGreaterThanZero");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .L("PageSize", "MustBeBetween1And100");

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .L("Search", "MaxLength", "100")
                .Must(BeValidSearchTerm)
                .When(x => !string.IsNullOrEmpty(x.Search))
                .L("Search", "InvalidCharacters");
        }

        private bool BeValidSearchTerm(string search)
        {
            if (string.IsNullOrEmpty(search)) return true;
            return Regex.IsMatch(search, @"^[a-zA-Z\u0600-\u06FF0-9\s\-_.]+$");
        }
    }
}
