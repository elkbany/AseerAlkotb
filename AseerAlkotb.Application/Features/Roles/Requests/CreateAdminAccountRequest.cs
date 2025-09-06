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
      string LastName,
      string UserName,
      string Email,
      string Password,
      string ConfirmPassword,
      string? Nationality,
      Gender Gender,
      string? PhoneNumber,
      string? Nationality,
      IFormFile? ProfilePictureUrl,
      string UserRole
    );
}
