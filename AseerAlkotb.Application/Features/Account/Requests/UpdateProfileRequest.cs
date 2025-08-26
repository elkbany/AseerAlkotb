using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace AseerAlkotb.Application.Features.Account.Requests
{
    public record UpdateProfileRequest
    (
        //int Id,
        string? FirstName,
        string? LastName,
        string? Bio,
        IFormFile? ProfilePictureUrl,
        string? Nationality,
        DateTime? DateOfBirth,
        Gender? Gender
    );
   
}
