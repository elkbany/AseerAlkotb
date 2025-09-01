using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace AseerAlkotb.Application.Features.Roles.Responses
{
    public record UpdateAdminAccountResponse
    (
        int Id,
        string FirstName,
        string LastName,
        string userName,
        string Email,
        string PhoneNumber,
        IFormFile ProfilePictureUrl,
        string Nationality,
        DateTime DateOfBirth,
        Gender Gender
    );
    
}
