using AseerAlkotb.Application.Features.Books.Mapping;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Books.Validators
{
    public class AddBookRequestValidator : AbstractValidator<AddBookRequest>
    {
        public AddBookRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title must be less than 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");

            RuleFor(x => x.ISBN)
                .NotEmpty().WithMessage("ISBN is required")
                .Matches(@"^\d{10}(\d{3})?$").WithMessage("ISBN must be 10 or 13 digits");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.DiscountPercentage)
                .InclusiveBetween(0, 100).WithMessage("Discount must be between 0 and 100");

            RuleFor(x => x.PublishedDate)
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Published date cannot be in the future");

            RuleFor(x => x.PageCount)
                .GreaterThan(0).WithMessage("Page count must be positive");

            RuleFor(x => x.Language)
                .NotEmpty().WithMessage("Language is required");

            RuleFor(x => x.CoverImageUrl)
                .Must(BeValidImage)
                .When(x => x.CoverImageUrl != null)
                .WithMessage("Image must be a valid image file (jpg, jpeg, png, gif) and less than 5MB");

            RuleFor(x => x.Format)
            .NotEmpty().WithMessage("Format is required");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock must be >= 0");

            RuleFor(x => x.AuthorId)
                .GreaterThan(0).WithMessage("Author is required");

            RuleFor(x => x.PublisherId)
                .GreaterThan(0).WithMessage("Publisher is required");

            RuleFor(x => x.CategoryIds)
                .NotEmpty().WithMessage("At least one category must be selected");
        
        }
        
        private bool BeValidImage(IFormFile? image)
        {
            if (image == null) return true; // Optional image

            // Check file size (5MB max)
            if (image.Length > 5 * 1024 * 1024)
                return false;

            // Check file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return false;

            // Check content type
            var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedContentTypes.Contains(image.ContentType.ToLowerInvariant()))
                return false;

            return true;
        }



    }
    
}
