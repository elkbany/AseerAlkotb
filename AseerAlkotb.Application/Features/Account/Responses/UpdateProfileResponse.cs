using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Account.Responses
{
    public record UpdateProfileResponse
    ( 
        int Id,
        string FirstName,
        string LastName,
        string Bio,
        string ProfilePictureUrl,
        string Nationality,
        DateTime DateOfBirth,
        Gender Gender

    );
}
