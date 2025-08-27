using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Account.Requests;
using AseerAlkotb.Application.Features.Authors.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace AseerAlkotb.Application.Features.Account.Validator
{
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator() 
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().When(x => x.FirstName != null) 
                .WithMessage("First name must not be empty.")
                .MaximumLength(20).When(x => x.FirstName != null)
                .WithMessage("First name must be less than 20 characters.")
                .Matches(@"^[a-zA-Z\s\-']+$").When(x => !string.IsNullOrEmpty(x.FirstName))
                .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes.");

            RuleFor(x => x.LastName)
                .NotEmpty().When(x => x.LastName != null)
                .WithMessage("Last name must not be empty.")
                .MaximumLength(20).When(x => x.LastName != null)
                .WithMessage("Last name must be less than 20 characters.")
                .Matches(@"^[a-zA-Z\s\-']+$").When(x => !string.IsNullOrEmpty(x.LastName))
                .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes.");

            RuleFor(x => x.Bio)
                .MaximumLength(500).When(x => x.Bio != null)
                .WithMessage("Bio must be less than 500 characters.");

            RuleFor(x => x.ProfilePictureUrl)
                .Must(BeValidImage)
                .When(x => x.ProfilePictureUrl != null)
                .WithMessage("Image must be a valid image file (jpg, jpeg, png, gif) and less than 5MB");

            //Rule for Nationality //make it enum and make dataType Enum???

           RuleFor(x => x.Nationality)
                .MaximumLength(30).When(x => x.Nationality != null)
                .WithMessage("Nationality must be between 10 and 2000 characters");

            // Rule for DateOfBirth
            RuleFor(x => x.DateOfBirth)
                .Must(BeInThePast).When(x => x.DateOfBirth.HasValue)
                .WithMessage("Date of birth must be a date in the past.")
                .Must(BeOldEnough).When(x => x.DateOfBirth.HasValue)
                .WithMessage("You must be at least 11 years old.");
        }

        

        // Custom validation method for date in the past
        private bool BeInThePast(DateTime? date)
        {
            if (!date.HasValue) return true;
            return date.Value < DateTime.UtcNow;
        }

        // Custom validation method for minimum age
        private bool BeOldEnough(DateTime? date)
        {
            if (!date.HasValue) return true;
            return date.Value <= DateTime.UtcNow.AddYears(-11);
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
