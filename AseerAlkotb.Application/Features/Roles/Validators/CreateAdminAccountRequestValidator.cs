using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Roles.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Roles.Validators
{
    public class CreateAdminAccountRequestValidator:AbstractValidator<CreateAdminAccountRequest>
    {
        public CreateAdminAccountRequestValidator() 
        {
            // Rule for FirstName
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .Length(3, 20).WithMessage("First name must be between {3} and {20} characters.")
                .Matches(@"^[a-zA-Z]+$").WithMessage("First name can only contain letters");

            // Rule for LastName
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .Length(3, 20).WithMessage("Last name must be between {3} and {20} characters.")
                .Matches(@"^[a-zA-Z]+$").WithMessage("First name can only contain letters");

            // Rule for UserName
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .Length(3, 20).WithMessage("Username must be between {MinLength} and {MaxLength} characters.")
                .Matches(@"^[a-zA-Z0-9_\.]+$").WithMessage("Username can only contain letters, numbers, underscores, and periods.");

            // Rule for Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(256).WithMessage("Email address cannot exceed {MaxLength} characters.");

            // Rule for Password
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least {MinLength} characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches(@"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-]").WithMessage("Password must contain at least one special character.");

            // Rule for ConfirmPassword
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Please confirm your password.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");

            // Rule for Gender
            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Please select a valid gender.");


            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number cannot be empty if provided.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Please provide a valid international phone number.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber)); ////my be need to change

        }
    }
    
}
