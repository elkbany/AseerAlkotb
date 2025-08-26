using AseerAlkotb.Application.Features.Books.Requests;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace AseerAlkotb.Application.Features.Books.Validators
{
    public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
    {
        public UpdateBookRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .L("Book", "Id", "MustBeGreaterThanZero");

            RuleFor(x => x.Title)
                .NotEmpty().L("Book", "Title", "Required")
                .MaximumLength(150).L("Book", "Title", "MaxLength", "150");

            RuleFor(x => x.ISBN)
                .NotEmpty().L("Book", "ISBN", "Required")
                .Matches(@"^\d{10}(\d{3})?$").L("Book", "ISBN", "MustBe10Or13Digits");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).L("Book", "Price", "MustBePositive");

            RuleFor(x => x.DiscountPercentage)
                .InclusiveBetween(0, 100).L("Book", "Discount", "MustBeBetween", "0", "100");

            RuleFor(x => x.PublishedDate)
                .LessThanOrEqualTo(DateTime.UtcNow).L("Book", "PublishedDate", "CannotBeFuture");

            RuleFor(x => x.PageCount)
                .GreaterThan(0).L("Book", "PageCount", "MustBeGreaterThanZero");

            RuleFor(x => x.Language)
                .NotEmpty().L("Book", "Language", "Required");

            RuleFor(x => x.Format)
                .NotEmpty().L("Book", "Format", "Required");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).L("Book", "StockQuantity", "CannotBeNegative");

            RuleFor(x => x.AuthorId)
                .GreaterThan(0).L("Author", "Id", "Required");

            RuleFor(x => x.PublisherId)
                .GreaterThan(0).L("Publisher", "Id", "Required");

            RuleFor(x => x.CoverImageUrl)
                .Must(BeValidImage)
                .When(x => x.CoverImageUrl != null)
                .L("Book", "Image", "InvalidImage");

            RuleFor(x => x.CategoryIds)
                .NotNull().L("Category", "Required")
                .Must(ids => ids.Any()).L("Category", "AtLeastOneRequired");

            RuleFor(x => x.IsActive)
                .NotNull().L("Book", "IsActive", "Required");
        }

        private bool BeValidImage(IFormFile? image)
        {
            if (image == null) return true;

            if (image.Length > 5 * 1024 * 1024) return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension)) return false;

            var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedContentTypes.Contains(image.ContentType.ToLowerInvariant())) return false;

            return true;
        }
    }
}
