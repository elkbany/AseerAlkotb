using AseerAlkotb.Application.Features.Publishers.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using AseerAlkotb.Application.ResponseHandler; // للـ Extension L
using AseerAlkotb.Domain.Resources;
using Microsoft.Extensions.Localization;

namespace AseerAlkotb.Application.Features.Publishers.Validators
{
    public class AddPublisherRequestValidator : AbstractValidator<AddPublisherRequest>
    {
        public AddPublisherRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .L("Publisher", "name", "Required")
                .Length(2, 100)
                .L("Publisher", "name", "MustBeBetween", "2" , "100")
                .Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$")
                .L("Publisher", "name", "LettersOnly");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .L("Description", "MaxLength", "1000");

            RuleFor(x => x.LogoUrl)
                .Must(BeValidImage)
                .When(x => x.LogoUrl != null)
                .L("image", "Invalid");

            RuleFor(x => x.ContactEmail)
                .NotEmpty()
                .L("email", "Required")
                .EmailAddress()
                .L("email", "Invalid");
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
