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
using AseerAlkotb.Application.Contracts.External;
using AseerAlkotb.Application.Features.Account.Responses;
using AseerAlkotb.Application.Features.Account.Validator;
using Microsoft.AspNetCore.WebUtilities;
using AseerAlkotb.Application.Features.Account.Requests;
using Mapster;
using Microsoft.AspNetCore.Hosting;


namespace AseerAlkotb.Application.Services
{
    public class AdminServices : AppService, IAdminServices
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private readonly IUnitOfWork unitOfWork;
        public AdminServices(UserManager<User> _userManager, IEmailService _emailService, IUnitOfWork _unitOfWork, IServiceProvider serviceProvider, IHostEnvironment environment, IConfiguration _configuration) : base(serviceProvider, environment)
        {
            this.userManager = _userManager;
            this.configuration = _configuration;
            this.emailService = _emailService;
            this.unitOfWork = _unitOfWork;
        }

        public async Task<ApiResponse<CreateAdminAccountResponse>> createAdminAccount(CreateAdminAccountRequest request)
        {
            try
            {
                // 1- Validate input
                await DoValidationAsync<CreateAdminAccountRequestValidator, CreateAdminAccountRequest>(request);

                // 2- Check for duplicate email
                if (await userManager.FindByEmailAsync(request.Email) is not null)
                    return BadRequest<CreateAdminAccountResponse>("Email is already taken");

                // 3- Check for duplicate username
                if (await userManager.FindByNameAsync(request.UserName) is not null)
                    return BadRequest<CreateAdminAccountResponse>("Username is already taken");

                // 4- Map request to User entity
                var newAccount = request.Adapt<User>();
                newAccount.CreatedAt = DateTime.UtcNow;
                newAccount.UpdatedAt = DateTime.UtcNow;
                newAccount.IsActive = true;

                // 5- Create user
                var result = await userManager.CreateAsync(newAccount, request.Password);
                if (!result.Succeeded)
                    return BadRequest<CreateAdminAccountResponse>(
                        string.Join(", ", result.Errors.Select(e => e.Description))
                    );

                //add role to the new Admin
                var addRole = await userManager.AddToRoleAsync(newAccount, "Admin");
                if (!addRole.Succeeded)
                {
                    return BadRequest<CreateAdminAccountResponse>("Failed to assign role to user");
                    //return BadRequest<RegisterResponse>(addRole.Errors.Select(e => e.Description).ToList());
                }

                // 6- Generate email confirmation token
                var token = await userManager.GenerateEmailConfirmationTokenAsync(newAccount);

                // 7- Encode token safely
                string tokenEncoded;
                try
                {
                    tokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"Token encoding failed: {ex.Message}");
                    return BadRequest<CreateAdminAccountResponse>("Failed to generate confirmation link");
                }

                // 8- Create confirmation URL
                //var frontendBase = _configuration["App:FrontendBaseUrl"] ?? "http://localhost:4200";
                var BackendBase = configuration["App:BackendBaseUrl"] ?? "http://localhost:5234";
                var confirmUrl = $"{BackendBase}/api/Account/confirm-email?userId={newAccount.Id}&token={tokenEncoded}";

                // 9- Send email
                var subject = "Confirm your email";
                var body = $@"
                    <p>Hello {newAccount.UserName},</p>
                    <p>Please confirm your email by clicking the link below:</p>
                    <p><a href=""{confirmUrl}"">Confirm Email</a></p>
                ";

                try
                {
                    await emailService.SendEmailAsync(newAccount.Email!, subject, body);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send confirmation email: {ex.Message}");
                    // retry mechanism 
                    return BadRequest<CreateAdminAccountResponse>("Failed to send confirmation email. Please try again later.");
                }

                var response = newAccount.Adapt<CreateAdminAccountResponse>();
                return Success(response, "Registered successfully. Please check your email to confirm.");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Unexpected error in Register: {ex.Message}");
                return BadRequest<CreateAdminAccountResponse>("An unexpected error occurred. Try again.");
            }
        }



        public async Task<ApiResponse<DeleteAdminAccountResponse>> DeleteAdminAccount(DeleteAdminAccountRequest request)
        {
            await DoValidationAsync<DeleteAdminAccountRequestValidator, DeleteAdminAccountRequest>(request);
            var user = await userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
            {
                return NotFound<DeleteAdminAccountResponse>("User not found");
            }
            
             // 3. Check if the user is an admin
            var isUserAdmin = await userManager.IsInRoleAsync(user, Roles.Admin.ToString());
            if (!isUserAdmin)
            {
                return BadRequest<DeleteAdminAccountResponse>("Cannot delete user. The specified user is not an administrator.");
            }
            var response =user.Adapt<DeleteAdminAccountResponse>();
            // 5. Delete the user
            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest<DeleteAdminAccountResponse>($"Failed to delete user: {errors}");
            }
            return Success(response);

        }




        public async Task<ApiResponse<UpdateAdminAccountResponse>> UpdateAdminAccount(int Id, UpdateAdminAccountRequest request)
        {
            await DoValidationAsync<UpdateAdminAccountRequestValidator, UpdateAdminAccountRequest>(request);

            var user = await userManager.FindByIdAsync(Id.ToString());
            if (user == null)
            {
                return NotFound<UpdateAdminAccountResponse>("User not found.");
            }

            // Check for email uniqueness 
            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                var userEmailExists = await userManager.FindByEmailAsync(request.Email);
                if (userEmailExists != null && userEmailExists.Id != user.Id)
                {
                    return BadRequest<UpdateAdminAccountResponse>("This email is already taken by another user.");
                }
            }

            // Check for username uniqueness
            if (!string.IsNullOrEmpty(request.UserName) && request.UserName != user.UserName)
            {
                var userNameExists = await userManager.FindByNameAsync(request.UserName);
                if (userNameExists != null && userNameExists.Id != user.Id)
                {
                    return BadRequest<UpdateAdminAccountResponse>("This username is already taken.");
                }
            }

            // Manual mapping 
            if (!string.IsNullOrEmpty(request.FirstName)) user.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName)) user.LastName = request.LastName;
            if (!string.IsNullOrEmpty(request.UserName)) user.UserName = request.UserName;
            if (!string.IsNullOrEmpty(request.Email)) user.Email = request.Email;
            if (!string.IsNullOrEmpty(request.PhoneNumber)) user.PhoneNumber = request.PhoneNumber;
            if (!string.IsNullOrEmpty(request.Nationality)) user.Nationality = request.Nationality;
            if (request.DateOfBirth.HasValue) user.DateOfBirth = request.DateOfBirth.Value;
            if (request.Gender.HasValue) user.Gender = request.Gender.Value;

            // Handle Profile Picture Upload 
            if (request.ProfilePictureUrl != null && request.ProfilePictureUrl.Length > 0)
            {
                user.ProfilePictureUrl = await UpdateImageAsync(
                    request.ProfilePictureUrl,
                    user.ProfilePictureUrl,
                    "user"
                );
            }

            // Update the UpdatedAt 
            user.UpdatedAt = DateTime.UtcNow;

            // Save changes 
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest<UpdateAdminAccountResponse>($"Failed to update user: {errors}");
            }

            var response = user.Adapt<UpdateAdminAccountResponse>();
            return Success(response, "Account updated successfully.");
        }




        public async Task<ApiResponse<List<GetAllClientResponse>>> GetAllClients()
        {
          
            var clients = await userManager.GetUsersInRoleAsync(Roles.Client.ToString());

            var response = clients.Adapt<List<GetAllClientResponse>>();

            return Success(response);
        }

        public async Task<ApiResponse<List<GetAllAdminResponse>>> GetAllAdmins()
        {

            var clients = await userManager.GetUsersInRoleAsync(Roles.Admin.ToString());

            var response = clients.Adapt<List<GetAllAdminResponse>>();

            return Success(response);
        }



        // Assign role
        public async Task<ApiResponse<AssignRoleResponse>> AssignRole(AssignRoleRequest request)
        {
            await DoValidationAsync<AssignRoleRequestValidator, AssignRoleRequest>(request);

            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return NotFound<AssignRoleResponse>("User not found");
            }

            var newRoleName = request.Role;

            // Check if the requested role is either Admin or Client
            if (newRoleName != Roles.Admin && newRoleName != Roles.Client)
            {
                return BadRequest<AssignRoleResponse>("Only Admin or Client roles can be assigned");
            }
            //check if has this role
            var userAlreadyInRole = await userManager.IsInRoleAsync(user, newRoleName.ToString());
            if (userAlreadyInRole)
            {
                return BadRequest<AssignRoleResponse>("User already has the requested role");
            }

            var currentUserRoles = await userManager.GetRolesAsync(user);

            var removeRole = await userManager.RemoveFromRolesAsync(user, currentUserRoles);
            if (!removeRole.Succeeded)
            {
                return BadRequest<AssignRoleResponse>("Failed to remove existing user roles. Assignment aborted.");
            }

            // Assign the new single role
            var addResult = await userManager.AddToRoleAsync(user, newRoleName.ToString());
            if (!addResult.Succeeded)
            {
                // restore previous roles
                var rollbackResult = await userManager.AddToRolesAsync(user, currentUserRoles);
                return BadRequest<AssignRoleResponse>("Failed to assign new rol but Still have previous role");
            }

            var response = new AssignRoleResponse(
                UserId: user.Id,
                newRole: newRoleName
            );

            return Success(response, "Role assigned successfully");
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
