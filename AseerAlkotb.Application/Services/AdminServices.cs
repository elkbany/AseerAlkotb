using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Contracts;
//using AseerAlkotb.Application.Features.Account.Responses;
using AseerAlkotb.Application.Features.Roles.Requests;
using AseerAlkotb.Application.Features.Roles.Responses;
using AseerAlkotb.Application.Features.Roles.Validators;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using AseerAlkotb.Domain.Enums;

using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;
using System.Collections.Frozen;


namespace AseerAlkotb.Application.Services
{
    public class AdminServices : AppService, IAdminServices
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        public AdminServices(UserManager<User> _userManager, IUnitOfWork _unitOfWork, IServiceProvider serviceProvider, IHostEnvironment environment, IConfiguration _configuration) : base(serviceProvider, environment)
        {
            this.userManager = _userManager;
            this.configuration = _configuration;
            this.unitOfWork = _unitOfWork;
        }

        //assgin role
        public async Task<ApiResponse<AssignRoleResponse>> AssignRole(AssignRoleRequest request)
        {
            await DoValidationAsync<AssignRoleRequestValidator, AssignRoleRequest>(request);
            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return NotFound<AssignRoleResponse>("User not found");
            }
            var newRoleName = request.Role;
            //check if newRole exisiting
            var userAlreadyInRole = await userManager.IsInRoleAsync(user, newRoleName.ToString());
            if (userAlreadyInRole)
            {
                return BadRequest<AssignRoleResponse>("User already has the requested role");
            }

            if (newRoleName ==Roles.Client)
            {
                return BadRequest<AssignRoleResponse>("Can't assign this requested role");
            }

            if (newRoleName == Roles.Admin || newRoleName == Roles.Staff)
            {
                if (await userManager.IsInRoleAsync(user, "Client"))
                {
                    var removeResult = await userManager.RemoveFromRoleAsync(user, "Client");
                }
            }

            var result = await userManager.AddToRoleAsync(user, newRoleName.ToString());

            if (!result.Succeeded)
            {
                return BadRequest<AssignRoleResponse>("Failed to assign role or this is assign before");
            }
            var response = new AssignRoleResponse(
              UserId: user.Id,
              newRole: newRoleName
            );

            return Success(response);
        }


        public async Task<ApiResponse<RemoveRoleResponse>> RemoveRole(RemoveRoleRequest request)
        {
            await DoValidationAsync<RemoveRoleRequestValidator, RemoveRoleRequest>(request);

            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return NotFound<RemoveRoleResponse>("User not found");

            if (request.Role == Roles.Client)
            {
                return BadRequest<RemoveRoleResponse>("Can't remove this requested role");
            }


            var OldRole = request.Role.ToString();
            // Check if user actually has this role
            var userHasRole = await userManager.IsInRoleAsync(user, OldRole);
            if (userHasRole==false)
            {
                return BadRequest<RemoveRoleResponse>("User does not have the specified role");
            }


            // Remove the role
            var result = await userManager.RemoveFromRoleAsync(user, OldRole);
            if (!result.Succeeded)
            {
                return BadRequest<RemoveRoleResponse>("Failed to remove role");
            }

            // If user has no roles left, assign them Client role
            var currentRoles = await userManager.GetRolesAsync(user);
            if (currentRoles.Count == 0)
            {
                //await userManager.DeleteAsync(user);
                await userManager.AddToRoleAsync(user, "Client");
                currentRoles = await userManager.GetRolesAsync(user);
            }

            var response = new RemoveRoleResponse(
                UserId: user.Id,
               CurrentRoles: currentRoles.ToList()
            );

            return Success(response);
        }
    }
}
