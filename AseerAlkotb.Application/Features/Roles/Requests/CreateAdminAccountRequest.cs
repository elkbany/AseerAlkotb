using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace AseerAlkotb.Application.Features.Roles.Requests
{
    public record CreateAdminAccountRequest
    (
      string FirstName,
      string? FirstName_en,
      string LastName,
      string? LastName_en,
      string UserName,
      string Email,
      string Password,
      string ConfirmPassword,
      string? Nationality,
      string? Nationality_en,
      Gender Gender,
      string? PhoneNumber,
      IFormFile? ProfilePictureUrl,
      string UserRole
    );
}
