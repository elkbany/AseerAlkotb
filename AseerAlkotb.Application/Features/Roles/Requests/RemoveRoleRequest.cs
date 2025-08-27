using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Roles.Requests
{
    public record RemoveRoleRequest(int UserId, Domain.Enums.Roles Role);


}
