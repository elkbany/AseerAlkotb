using AseerAlkotb.Application.Features.Publishers.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using AseerAlkotb.Localization.Resources;
using Microsoft.Extensions.Localization;
using AseerAlkotb.Application.ResponseHandler;

namespace AseerAlkotb.Application.Features.Publishers.Validators
{
    public class UpdatePublisherRequestValidator : AbstractValidator<UpdatePublisherRequest>
    {
        public UpdatePublisherRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .L("Publisher", "Id" , "GreaterThanZero");

            RuleFor(x => x.Name)
                .NotEmpty()
                .L("Publisher", "Name" , "Required")
                .Length(2, 200)
                .L("Publisher", "Name" , "Length")
                .Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$")
                .L("Publisher", "Name" , "LettersOnly");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .L("Description", "MaxLength1000");

            RuleFor(x => x.LogoUrl)
                .Must(BeValidImage)
                .L("Publisher", "image", "invalid");



            RuleFor(x => x.ContactEmail)
                .NotEmpty()
                .L("Publisher", "EmailRequired")
                .EmailAddress()
                .L("Publisher", "EmailInvalid");
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
