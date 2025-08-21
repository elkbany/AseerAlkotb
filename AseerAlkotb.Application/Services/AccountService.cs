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
using AseerAlkotb.Application.Features.CartItems.Validation;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class AccountService :AppService ,IAccountServices
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        public AccountService(UserManager<User> _userManager,IServiceProvider serviceProvider, IHostEnvironment environment,IConfiguration _configuration) : base(serviceProvider, environment)
        { 
            this.userManager = _userManager;
            this.configuration = _configuration;
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
            var result= await userManager.CreateAsync(newAccount,request.Password);
            if (!result.Succeeded) {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest<RegisterResponse>("Try Again");
                //return BadRequest<RegisterResponse>(errors);
            }
            var response= newAccount.Adapt<RegisterResponse>();
            return Success(response);

        }


        public async Task<ApiResponse<LoginResponse>> Login(LoginRequest request)
        {
            await DoValidationAsync<LoginRequestValidator, LoginRequest>(request);
            //var normalizedEmail = userManager.NormalizeEmail(request.Email);
            //var existingUser = await userManager.FindByEmailAsync(normalizedEmail);
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
    }
}
