using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Account.Requests;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Domain.Entites.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace AseerAlkotb.Application.Features.Account.Validator
{
   public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator() 
        { 
            RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(2, 15).WithMessage("First name must be between 2-15 characters")
            .Matches(@"^[a-zA-Z]+$").WithMessage("First name can only contain letters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .Length(2, 15).WithMessage("Last name must be between 2-15 characters")
                .Matches(@"^[a-zA-Z]+$").WithMessage("Last name can only contain letters");

            RuleFor(x => x.UserName)
               .NotEmpty().WithMessage("Username is required")
               .Length(3, 20).WithMessage("Username must be between 3-20 characters")
               .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email is required")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .MaximumLength(20).WithMessage("Password cannot exceed 20 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
                .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords must match")
                .When(x => !string.IsNullOrEmpty(x.Password));
        }




       
    }
}
