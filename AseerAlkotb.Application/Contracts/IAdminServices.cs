using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Roles.Requests;
using AseerAlkotb.Application.Features.Roles.Responses;
using AseerAlkotb.Application.ResponseHandler;

namespace AseerAlkotb.Application.Contracts
{
    public interface IAdminServices
    {
        Task<ApiResponse<AssignRoleResponse>> AssignRole(AssignRoleRequest request);
        Task<ApiResponse<RemoveRoleResponse>> RemoveRole(RemoveRoleRequest request);
    }
}
