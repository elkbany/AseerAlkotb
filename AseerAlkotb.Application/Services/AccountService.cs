using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Contracts.External;
using AseerAlkotb.Application.Features.Account.Requests;
using AseerAlkotb.Application.Features.Account.Responses;
using AseerAlkotb.Application.Features.Account.Validator;
using AseerAlkotb.Application.Features.CartItem.Requests;
using AseerAlkotb.Application.Features.CartItems.Validation;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class AccountService : AppService, IAccountServices
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AccountService(UserManager<User> userManager, IServiceProvider serviceProvider, IHostEnvironment environment, IConfiguration configuration, IEmailService emailService) : base(serviceProvider, environment)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<ApiResponse<RegisterResponse>> CreateAccount(RegisterRequest request)
        {
            await DoValidationAsync<RegisterRequestValidator, RegisterRequest>(request);
            var existingUserName = await _userManager.FindByNameAsync(request.UserName);
            if (existingUserName != null)
            {
                return BadRequest<RegisterResponse>("Username is already taken");
            }
            var newAccount = request.Adapt<User>();
            var result = await _userManager.CreateAsync(newAccount, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest<RegisterResponse>("Try Again");
                //return BadRequest<RegisterResponse>(errors);
            }
            var response = newAccount.Adapt<RegisterResponse>();
            return Success(response);

        }

        public async Task<ApiResponse<RegisterResponse>> Register(RegisterRequest request)
        {
            try
            {
                // 1- Validate input
                await DoValidationAsync<RegisterRequestValidator, RegisterRequest>(request);

                // 2- Check for duplicate email
                if (await _userManager.FindByEmailAsync(request.Email) is not null)
                    return BadRequest<RegisterResponse>("Email is already taken");

                // 3- Check for duplicate username
                if (await _userManager.FindByNameAsync(request.UserName) is not null)
                    return BadRequest<RegisterResponse>("Username is already taken");

                // 4- Map request to User entity
                var newAccount = request.Adapt<User>();

                // 5- Create user
                var result = await _userManager.CreateAsync(newAccount, request.Password);
                if (!result.Succeeded)
                    return BadRequest<RegisterResponse>(
                        string.Join(", ", result.Errors.Select(e => e.Description))
                    );

                // 6- Generate email confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newAccount);

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
                    return BadRequest<RegisterResponse>("Failed to generate confirmation link");
                }

                // 8- Create confirmation URL
                //var frontendBase = _configuration["App:FrontendBaseUrl"] ?? "http://localhost:4200";
                var BackendBase = _configuration["App:BackendBaseUrl"] ?? "http://localhost:5234";
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
                    await _emailService.SendEmailAsync(newAccount.Email!, subject, body);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send confirmation email: {ex.Message}");
                    // retry mechanism 
                    return BadRequest<RegisterResponse>("Failed to send confirmation email. Please try again later.");
                }

                var response = newAccount.Adapt<RegisterResponse>();
                return Success(response, "Registered successfully. Please check your email to confirm.");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Unexpected error in Register: {ex.Message}");
                return BadRequest<RegisterResponse>("An unexpected error occurred. Try again.");
            }
        }


        public async Task<ApiResponse<string>> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            string decodedToken;
            try
            {
                var decodedBytes = WebEncoders.Base64UrlDecode(token);
                decodedToken = Encoding.UTF8.GetString(decodedBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token decoding failed: {ex.Message}");
                return BadRequest<string>("Invalid token format");
            }

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded) return BadRequest<string>("Invalid or expired token");

            return Success("Email confirmed successfully");
        }

        public async Task<ApiResponse<string>> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Success("If an account with this email exists, a password reset link has been sent.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var tokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            //var frontendBase = _configuration["App:FrontendBaseUrl"] ?? "http://localhost:4200";
            var BackendBase = _configuration["App:BackendBaseUrl"] ?? "http://localhost:5234";
            var resetUrl = $"{BackendBase}/api/Account/reset-password?userId={user.Id}&token={tokenEncoded}";

            var subject = "Reset Your Password";
            var body = $@"
        <p>Hello {user.UserName},</p>
        <p>Click the link below to reset your password:</p>
        <p><a href=""{resetUrl}"">Reset Password</a></p>
        <p>If you didn't request this, you can ignore this email.</p>
    ";

            try
            {
                await _emailService.SendEmailAsync(user.Email!, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send reset password email: {ex.Message}");
                return BadRequest<string>("Failed to send reset password email. Please try again later.");
            }

            return Success("If an account with this email exists, a password reset link has been sent.");
        }

        public async Task<ApiResponse<string>> ResetPassword(ResetPasswordRequest request)
        {
            // Validate request
            await DoValidationAsync<ResetPasswordRequestValidator, ResetPasswordRequest>(request);

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound("User not found");

            string decodedToken;
            try
            {
                var decodedBytes = WebEncoders.Base64UrlDecode(request.Token);
                decodedToken = Encoding.UTF8.GetString(decodedBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token decoding failed: {ex.Message}");
                return BadRequest<string>("Invalid token format");
            }

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest<string>(errors);
            }

            return Success("Password reset successfully.");
        }



        //    public async Task<ApiResponse<LoginResponse>> Login(LoginRequest request)
        //    {
        //        await DoValidationAsync<LoginRequestValidator, LoginRequest>(request);
        //        //var normalizedEmail = userManager.NormalizeEmail(request.Email);
        //        //var existingUser = await userManager.FindByEmailAsync(normalizedEmail);
        //        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        //        if (existingUser != null)
        //        {

        //            bool foundPassword = await _userManager.CheckPasswordAsync(existingUser, request.Password);
        //            if (foundPassword == true)
        //            {
        //                List<Claim>userClaims = new List<Claim>();
        //                userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()));
        //                userClaims.Add(new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()));
        //                userClaims.Add(new Claim(ClaimTypes.Name, existingUser.UserName));
        //                var userRole=(await _userManager.GetRolesAsync(existingUser)).ToList();
        //                foreach (var role in userRole) { 
        //                    userClaims.Add(new Claim(ClaimTypes.Role,role.ToString()));
        //                }

        //                var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
        //                SigningCredentials signingCredentials=new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256);

        //                //designToken
        //                JwtSecurityToken token = new JwtSecurityToken(

        //                    issuer: _configuration["JWT:IssuerIP"],
        //                    audience: _configuration["JWT:AudienceIP"],
        //                    expires: DateTime.UtcNow.AddDays(7),
        //                    claims:userClaims,
        //                    signingCredentials: signingCredentials

        //                );

        //                var response = new LoginResponse()
        //                {
        //                    Id= existingUser.Id,
        //                    Token =new JwtSecurityTokenHandler().WriteToken(token),
        //                    Expiration= DateTime.UtcNow.AddDays(7),
        //                };
        //                return Success(response);
        //            }
        //            else
        //            {
        //                return BadRequest<LoginResponse>("Email or Password Invalid");
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest<LoginResponse>("Email or Password Invalid");
        //        }

        //    }
        //}
        public async Task<ApiResponse<LoginResponse>> Login(LoginRequest request)
        {
            try
            {
                await DoValidationAsync<LoginRequestValidator, LoginRequest>(request);

                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser == null)
                    return BadRequest<LoginResponse>("Email or Password invalid");

                // Check if email is confirmed
                if (!existingUser.EmailConfirmed)
                    return BadRequest<LoginResponse>("Please confirm your email before logging in");

                bool validPassword = await _userManager.CheckPasswordAsync(existingUser, request.Password);
                if (!validPassword)
                    return BadRequest<LoginResponse>("Email or Password invalid");

                // Create user claims
                var userClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
            new Claim(ClaimTypes.Name, existingUser.UserName)
        };

                var roles = await _userManager.GetRolesAsync(existingUser);
                userClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                // JWT Token
                var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
                var signingCredentials = new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:IssuerIP"],
                    audience: _configuration["JWT:AudienceIP"],
                    expires: DateTime.UtcNow.AddDays(7),
                    claims: userClaims,
                    signingCredentials: signingCredentials
                );

                var response = new LoginResponse
                {
                    Id = existingUser.Id,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                };

                return Success(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in Login: {ex.Message}");
                return BadRequest<LoginResponse>("An unexpected error occurred. Try again.");
            }
        }

        // ResendEmailConfirmation with email
        public async Task<ApiResponse<string>> ResendEmailConfirmation(string email)
        {
            try
            {
                // Validate email format
                if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                    return BadRequest<string>("Invalid email format");
                // Find user by email
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return NotFound<string>("User not found");
                // Check if email is already confirmed
                if (user.EmailConfirmed)
                    return BadRequest<string>("Email is already confirmed");
                // Generate confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string tokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                // Create confirmation URL
                //var frontendBase = _configuration["App:FrontendBaseUrl"] ?? "http://localhost:4200";
                var BackendBase = _configuration["App:BackendBaseUrl"] ?? "http://localhost:5234";
                var confirmUrl = $"{BackendBase}/api/Account/confirm-email?userId={user.Id}&token={tokenEncoded}";
                // Send confirmation email
                var subject = "Confirm your email";
                var body = $@"
            <p>Hello {user.UserName},</p>
            <p>Please confirm your email by clicking the link below:</p>
            <p><a href=""{confirmUrl}"">Confirm Email</a></p>";
                try
                {
                    await _emailService.SendEmailAsync(user.Email!, subject, body);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send confirmation email: {ex.Message}");
                    return BadRequest<string>("Failed to send confirmation email. Please try again later.");
                }
                return Success("Confirmation email sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in ResendEmailConfirmation: {ex.Message}");
                return BadRequest<string>("An unexpected error occurred. Try again.");


            }
        }
    }
}
