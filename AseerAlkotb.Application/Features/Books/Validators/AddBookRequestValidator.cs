using AseerAlkotb.Application.Features.Books.Mapping;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace AseerAlkotb.Application.Features.Books.Validators
{
    public class AddBookRequestValidator : AbstractValidator<AddBookRequest>
    {
        public AddBookRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .L("Book", "Title", "Required")
                .MaximumLength(200)
                .L("Book", "Title", "MustBeLessThan", "200");

            RuleFor(x => x.Description)
                .NotEmpty()
                .L("Book", "Description", "Required");

            RuleFor(x => x.ISBN)
                .NotEmpty()
                .L("Book", "ISBN", "Required")
                .Matches(@"^\d{10}(\d{3})?$")
                .L("Book", "ISBN", "MustBeDigits", "10", "13");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .L("Book", "Price", "MustBeGreaterThan", "0");

            RuleFor(x => x.DiscountPercentage)
                .InclusiveBetween(0, 100)
                .L("Book", "Discount", "MustBeBetween", "0", "100");

            RuleFor(x => x.PublishedDate)
                .LessThanOrEqualTo(DateTime.Today)
                .L("Book", "PublishedDate", "CannotBeInFuture");

            RuleFor(x => x.PageCount)
                .GreaterThan(0)
                .L("Book", "PageCount", "MustBePositive");

            RuleFor(x => x.Language)
                .NotEmpty()
                .L("Book", "Language", "Required");

            RuleFor(x => x.CoverImageUrl)
                .Must(BeValidImage)
                .When(x => x.CoverImageUrl != null)
                .L("Book", "Image", "InvalidImage");

            RuleFor(x => x.Format)
                .NotEmpty()
                .L("Book", "Format", "Required");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0)
                .L("Book", "Stock", "MustBeGreaterOrEqual", "0");

            RuleFor(x => x.AuthorId)
                .GreaterThan(0)
                .L("Author", "Id", "Required");

            RuleFor(x => x.PublisherId)
                .GreaterThan(0)
                .L("Publisher", "Id", "Required");

            RuleFor(x => x.CategoryIds)
                .NotEmpty()
                .L("Category", "Id", "Required");
        }

        private bool BeValidImage(IFormFile? image)
        {
            if (image == null) return true;

            if (image.Length > 5 * 1024 * 1024)
                return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return false;

            var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedContentTypes.Contains(image.ContentType.ToLowerInvariant()))
                return false;

            return true;
        }
    }
}
