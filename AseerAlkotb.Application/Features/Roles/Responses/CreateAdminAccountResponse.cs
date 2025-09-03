using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Roles.Responses
{
    public record CreateAdminAccountResponse
    (
      int Id,
      string FirstName,
      string LastName,
      string UserName,
      string Email,
      Gender Gender,
      bool IsActive,
      string PhoneNumber,
      string Nationality
    );
}
