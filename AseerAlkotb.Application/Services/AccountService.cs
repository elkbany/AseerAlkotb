using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Account.Requests;
using AseerAlkotb.Application.Features.Account.Responses;
using AseerAlkotb.Application.Features.Account.Validator;
using AseerAlkotb.Application.Features.CartItem.Requests;
using AseerAlkotb.Application.Features.CartItem.Responses;
using AseerAlkotb.Application.Features.CartItems.Validation;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class AccountService :AppService ,IAccountServices
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        public AccountService(UserManager<User> _userManager, IUnitOfWork _unitOfWork, IServiceProvider serviceProvider, IHostEnvironment environment,IConfiguration _configuration) : base(serviceProvider, environment)
        { 
            this.userManager = _userManager;
            this.configuration = _configuration;
            this.unitOfWork = _unitOfWork;
        } 

        public async Task<ApiResponse<RegisterResponse>> CreateAccount(RegisterRequest request)
        {
            await DoValidationAsync<RegisterRequestValidator, RegisterRequest>(request);
            var existingUserName=await userManager.FindByNameAsync(request.UserName);
            if (existingUserName !=null)
            {
                return BadRequest<RegisterResponse>("Username is already taken");
            }
            var newAccount = request.Adapt<User>();
            newAccount.CreatedAt = DateTime.UtcNow;
            newAccount.UpdatedAt = DateTime.UtcNow;
            var result= await userManager.CreateAsync(newAccount,request.Password);
            if (!result.Succeeded) {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest<RegisterResponse>("Try Again");
                //return BadRequest<RegisterResponse>(errors);
            }

            //add role to the new user
            var addRole = await userManager.AddToRoleAsync(newAccount, "Client");
            if (!addRole.Succeeded)
            {
                return BadRequest<RegisterResponse>("Failed to assign role to user");
                //return BadRequest<RegisterResponse>(addRole.Errors.Select(e => e.Description).ToList());
            }

            //add cart for the new user
            var cart =new Cart()
            {
                UserId = newAccount.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await unitOfWork.Carts.InsertAsync(cart);
            await unitOfWork.CommitAsync();
            var response= newAccount.Adapt<RegisterResponse>();
            return Success(response);

        }


        public async Task<ApiResponse<LoginResponse>> Login(LoginRequest request)
        {
            await DoValidationAsync<LoginRequestValidator, LoginRequest>(request);
            
            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {

                bool foundPassword = await userManager.CheckPasswordAsync(existingUser, request.Password);
                if (foundPassword == true)
                {
                    List<Claim>userClaims = new List<Claim>();
                    userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()));
                    userClaims.Add(new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()));
                    userClaims.Add(new Claim(ClaimTypes.Name, existingUser.UserName));
                    var userRole=(await userManager.GetRolesAsync(existingUser)).ToList();
                    foreach (var role in userRole) { 
                        userClaims.Add(new Claim(ClaimTypes.Role,role.ToString()));
                    }

                    var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));
                    SigningCredentials signingCredentials=new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256);

                    //designToken
                    JwtSecurityToken token = new JwtSecurityToken(

                        issuer: configuration["JWT:IssuerIP"],
                        audience: configuration["JWT:AudienceIP"],
                        expires: DateTime.UtcNow.AddDays(7),
                        claims:userClaims,
                        signingCredentials: signingCredentials

                    );

                    var response = new LoginResponse()
                    {
                        Id= existingUser.Id,
                        Token =new JwtSecurityTokenHandler().WriteToken(token),
                        Expiration= DateTime.UtcNow.AddDays(7),
                    };
                    return Success(response);
                }
                else
                {
                    return BadRequest<LoginResponse>("Email or Password Invalid");
                }
            }
            else
            {
                return BadRequest<LoginResponse>("Email or Password Invalid");
            }

        }

        public async Task<ApiResponse<UpdateProfileResponse>> UpdateProfile( int userId,UpdateProfileRequest request)
        {
            await DoValidationAsync<UpdateProfileRequestValidator, UpdateProfileRequest>(request);
            var existingUser = await userManager.FindByIdAsync(userId.ToString());
            if (existingUser == null)
            {
                return NotFound<UpdateProfileResponse>("User not found");
            }
            existingUser = request.Adapt(existingUser);
            var result = await userManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
            {
                return BadRequest<UpdateProfileResponse>("Try Again");
            }
            var response = existingUser.Adapt<UpdateProfileResponse>();
            return Success(response);
        }
      
        public async Task<ApiResponse<GetProfileResponse>>GetProfile(GetProfileRequest request)
        {
            await DoValidationAsync<GetProfileRequestValidator,GetProfileRequest>(request);

            var existingUser = await unitOfWork.Account.GetUserWithRelatedData(request.UserId);
            if (existingUser == null)
            {
                return NotFound<GetProfileResponse>("User not found");
            }
            var response = new GetProfileResponse()
            {
                Id = existingUser.Id,
                FirstName = existingUser.FirstName,
                LastName = existingUser.LastName,
                ImageUrl=existingUser.ProfilePictureUrl,
                RegistrationPeriod= DateTime.UtcNow-existingUser.CreatedAt,
                Reviews=existingUser.Reviews?.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    ReviewFor = r.ReviewFor
                }).ToList()??new List<ReviewDto>(),

                Following = existingUser.Following?.Select(f => new UserFollowDto
                {
                    Id = f.Id,
                    FollowType = f.FollowType
                }).ToList() ?? new List<UserFollowDto>()
            };
            return Success(response);

        }

       
    }
}
