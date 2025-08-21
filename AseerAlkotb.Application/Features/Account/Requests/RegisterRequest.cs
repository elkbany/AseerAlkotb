using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Account.Requests
{
    public record RegisterRequest
    (
      string FirstName,
      string LastName,
      string UserName,
      string Email,
      string Password,
      string ConfirmPassword

    );
    
}
