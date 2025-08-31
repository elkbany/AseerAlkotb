using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Roles.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace AseerAlkotb.Application.Features.Roles.Validators
{
    public class UpdateAdminAccountRequestValidator :AbstractValidator<UpdateAdminAccountRequest>
    {
        public UpdateAdminAccountRequestValidator()
        {
            // Rule for FirstName (Optional - only validate if provided)
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name cannot be empty if provided.")
                .Length(3, 15).WithMessage("First name must be between {3} and {15} characters.")
                .Matches(@"^[\p{L}\s\-']+$").WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes.")
                .When(x => !string.IsNullOrEmpty(x.FirstName)); // Validate ONLY if provided

            // Rule for LastName (Optional - only validate if provided)
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name cannot be empty if provided.")
                .Length(3, 15).WithMessage("Last name must be between {3} and {15} characters.")
                .Matches(@"^[\p{L}\s\-']+$").WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes.")
                .When(x => !string.IsNullOrEmpty(x.LastName)); // Validate ONLY if provided

            // Rule for UserName (Optional - only validate if provided)
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username cannot be empty if provided.")
                .Length(3, 20).WithMessage("Username must be between {3} and {20} characters.")
                .Matches(@"^[a-zA-Z0-9_\.]+$").WithMessage("Username can only contain letters, numbers, underscores, and periods.")
                .When(x => !string.IsNullOrEmpty(x.UserName)); // Validate ONLY if provided

            // Rule for Email (Optional - only validate if provided)
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(256).WithMessage("Email address cannot exceed {MaxLength} characters.")
                .When(x => !string.IsNullOrEmpty(x.Email)); // Validate ONLY if provided

            // Rule for PhoneNumber (Optional - only validate if provided)
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number cannot be empty if provided.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Please provide a valid international phone number.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber)); // Validate ONLY if provided

            // Rule for ProfilePictureUrl (Optional - only validate if provided)
            RuleFor(x => x.ProfilePictureUrl)
               .Must(BeValidImage).WithMessage("Profile image must be a valid image file (JPEG, PNG, GIF) and less than 5MB.")
               .When(x => x.ProfilePictureUrl != null); // Validate ONLY if provided

            // Rule for Nationality (Optional - only validate if provided)
            RuleFor(x => x.Nationality)
                .NotEmpty().WithMessage("Nationality cannot be empty if provided.")
                .Length(2, 100).WithMessage("Nationality must be between {MinLength} and {MaxLength} characters.")
                .Matches(@"^[\p{L}\s\-\.,\(\)]+$").WithMessage("Nationality can only contain letters, spaces, hyphens, commas, and parentheses.")
                .When(x => !string.IsNullOrEmpty(x.Nationality)); // Validate ONLY if provided

            // Rule for DateOfBirth (Optional - only validate if provided)
            RuleFor(x => x.DateOfBirth)
                .Must(BeInThePast).WithMessage("Date of birth must be in the past.")
                .Must(BeOldEnough).WithMessage("User must be at least 11 years old.")
                .When(x => x.DateOfBirth.HasValue); // Validate ONLY if provided

            // Rule for Gender (Optional - only validate if provided)
            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Please select a valid gender.")
                .When(x => x.Gender.HasValue); // Validate ONLY if provided
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
