using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Localization.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace AseerAlkotb.Application.Features.Authors.Validators
{
    public class AddAuthorRequestValidator : AbstractValidator<AddAuthorRequest>
    {
        public AddAuthorRequestValidator()
        {

            RuleFor(x => x.Name)
               .NotEmpty()
               .L("Author").L("Name").L("Required")
               .Length(2, 200)
               .L("Author").L("Name").L("MustBeBetween", "2", "200")
               .Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$")
               .L("Author").L("Name").L("LettersOnly");


            RuleFor(x => x.Bio)
              .NotEmpty()
              .L("Author").L("Bio").L("Required")
              .Length(10, 2000)
              .L("Author").L("Bio").L("MustBeBetween", "10", "2000");

            RuleFor(x => x.Image)
                .Must(BeValidImage)
                .When(x => x.Image != null)
                .L("Author").L("Image").L("InvalidImage");


            RuleFor(x => x.CountryCode)
                 .IsInEnum()
                 .L("Author", "CountryCode", "Invalid");

        }

        private bool BeValidImage(IFormFile? image)
        {
            if (image == null) return true; // Optional image

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
