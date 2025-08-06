using AseerAlkotb.Application.Features.Publishers.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Publishers.Validators
{
    public class UpdatePublisherRequestValidator : AbstractValidator<UpdatePublisherRequest>
    {
        public UpdatePublisherRequestValidator() 
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Publisher ID must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Publisher name is required")
                .Length(2, 200)
                .WithMessage("Publisher name must be between 2 and 200 characters")
                .Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$")
                .WithMessage("Publisher name can only contain letters and spaces");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters");

            RuleFor(x => x.LogoUrl)
                .NotEmpty()
                .WithMessage("Logo URL is required")
                .Must(BeValidImage)
                .WithMessage("Logo must be a valid image file (jpg, jpeg, png, gif) and less than 5MB");

            RuleFor(x => x.ContactEmail)
                .NotEmpty()
                .WithMessage("Contact email is required")
                .EmailAddress()
                .WithMessage("A valid email address is required");
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
