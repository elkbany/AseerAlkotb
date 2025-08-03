using AseerAlkotb.Application.Features.Books.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Books.Validators
{
    public class FilterBooksRequestValidator : AbstractValidator<FilterBooksRequest>
    {
        public FilterBooksRequestValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.SortBy)
                .IsInEnum()
                .When(x => x.SortBy.HasValue)
                .WithMessage("Invalid sort option.");
        }
    }
}
