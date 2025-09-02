using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Account.Requests;
using AseerAlkotb.Application.Features.Account.Responses;
using AseerAlkotb.Application.Features.Roles.Requests;
using AseerAlkotb.Application.Features.Roles.Responses;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace AseerAlkotb.Application.Contracts
{
    public interface IAdminServices
    {
        Task<ApiResponse<AssignRoleResponse>> AssignRole(AssignRoleRequest request);
        Task<ApiResponse<RemoveRoleResponse>> RemoveRole(RemoveRoleRequest request);
        Task<ApiResponse<CreateAdminAccountResponse>> createAdminAccount(CreateAdminAccountRequest request);
        Task<ApiResponse<DeleteAdminAccountResponse>> DeleteAdminAccount(DeleteAdminAccountRequest request);
        Task<ApiResponse<UpdateAdminAccountResponse>> UpdateAdminAccount(int Id, UpdateAdminAccountRequest request);
        Task<ApiResponse<List<GetAllClientResponse>>> GetAllClients();
        Task<ApiResponse<List<GetAllAdminResponse>>> GetAllAdmins();
    }
}
