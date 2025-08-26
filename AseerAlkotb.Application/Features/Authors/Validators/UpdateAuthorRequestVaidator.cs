using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace AseerAlkotb.Application.Features.Authors.Validators
{
    public class UpdateAuthorRequestValidator : AbstractValidator<UpdateAuthorRequest>
    {
        public UpdateAuthorRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .L("Author", "Id", "MustBeGreaterThan", "0");

            RuleFor(x => x.Name)
                .NotEmpty()
                .L("Author", "Name", "Required")
                .Length(2, 200)
                .L("Author", "Name", "MustBeBetween", "2", "200")
                .Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$")
                .L("Author", "Name", "LettersOnly");

            RuleFor(x => x.Bio)
                .NotEmpty()
                .L("Author", "Bio", "Required")
                .Length(10, 2000)
                .L("Author", "Bio", "MustBeBetween", "10", "2000");

            RuleFor(x => x.Image)
                .Must(BeValidImage)
                .When(x => x.Image != null)
                .L("Author", "Image", "InvalidImage");

            RuleFor(x => x.CountryCode)
                .IsInEnum()
                .L("Author", "CountryCode", "Invalid");
        }

        private bool BeValidImage(IFormFile? image)
        {
            if (image == null) return true;

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
