using AseerAlkotb.Application.Features.Quotes.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Quotes.Validators
{
    public class AddQuoteRequestValidator : AbstractValidator<AddQuoteRequest>
    {
       public AddQuoteRequestValidator()
       {
           RuleFor(x => x)
               .Must(x => (x.BookId.HasValue && !x.AuthorId.HasValue) ||
                          (!x.BookId.HasValue && x.AuthorId.HasValue))
               .WithMessage("Either BookId or AuthorId must be provided, but not both.");

           RuleFor(x => x.Comment)
               .NotEmpty().WithMessage("Comment is required.")
               .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.");
       }
    }
}
