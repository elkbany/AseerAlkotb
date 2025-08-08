using AseerAlkotb.Application.Features.Authors.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Http;


namespace AseerAlkotb.Application.Features.Authors.Validators
{
    public class AddAuthorRequestValidator : AbstractValidator<AddAuthorRequest>
    {
        public AddAuthorRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Author name is required")
                .Length(2, 200)
                .WithMessage("Author name must be between 2 and 200 characters")
                .Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$")
                .WithMessage("Author name can only contain letters and spaces");

            RuleFor(x => x.Bio)
                .NotEmpty()
                .WithMessage("Author bio is required")
                .Length(10, 2000)
                .WithMessage("Bio must be between 10 and 2000 characters");

            RuleFor(x => x.Image)
                .Must(BeValidImage)
                .When(x => x.Image != null)
                .WithMessage("Image must be a valid image file (jpg, jpeg, png, gif) and less than 5MB");

            RuleFor(x => x.CountryCode)
                .IsInEnum()
                .WithMessage("Country code is invalid");
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
