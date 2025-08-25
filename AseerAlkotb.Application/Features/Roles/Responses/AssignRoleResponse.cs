using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Roles.Responses
{
    public record AssignRoleResponse(int UserId, Domain.Enums.Roles newRole);
}
