﻿using System;
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
        string? FirstName_en,
        string? LastName,
        string? LastName_en,
        string? Bio,
        string? Bio_en,
        IFormFile? ProfilePictureUrl,
        string? Nationality,
        string? Nationality_en,
        DateTime? DateOfBirth,
        Gender? Gender
    );
   
}
