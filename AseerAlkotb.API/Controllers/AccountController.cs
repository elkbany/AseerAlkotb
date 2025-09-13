using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Account.Requests;
using AseerAlkotb.Application.Services;
using AseerAlkotb.Domain.Entites.Models;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountServices;
        private readonly IConfiguration _configuration;
        public AccountController(IAccountServices accountServices, IConfiguration configuration) { 
           _accountServices = accountServices;
           _configuration = configuration;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _accountServices.Register(request);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Message = result.Message, // Use the actual message from the service
                    Errors = result.Errors
                });
            }
            return Ok(new
            {
                Message = result.Message, // Use the success message from the service
                Data = result.Data
            });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _accountServices.ConfirmEmail(userId, token);

            var frontendBase = _configuration["App:FrontendBaseUrl"] ?? "http://localhost:4200";

            if (!result.Succeeded)
            {
                // Redirect to a failure page in the frontend
                return Redirect($"{frontendBase}/confirm-email-failed");
            }

            // Redirect to a success page in the frontend
            return Redirect($"{frontendBase}/confirm-email-success");
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _accountServices.Login(request);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Message = "Login failed",
                    Errors = result.Errors
                });
            }

            return Ok(new
            {
                Message = "Login successful",
                Data = result.Data
            });
        }

        // Forgot Password
        [HttpPost("ForgotPassword/{email}")]
   
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _accountServices.ForgotPassword(email);

            if (!result.Succeeded)
                return BadRequest(new
                {
                    Success = false,
                    Message = "إذا كان هناك حساب بهذا البريد الإلكتروني، فقد تم إرسال رابط إعادة تعيين كلمة المرور.",
                    result.Errors
                });

            return Ok(new
            {
                Success = true,
                Message = "إذا كان هناك حساب بهذا البريد الإلكتروني، فقد تم إرسال رابط إعادة تعيين كلمة المرور.",
             
            });
        }
        // Reset Password
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _accountServices.ResetPassword(request);
            if (!result.Succeeded)
                return BadRequest(new { Message = result.Message });

            return Ok(new { Message = result.Message });
        }

        [HttpGet("reset-password")]
        public IActionResult ResetPasswordPage([FromQuery] string userId, [FromQuery] string token)
        {
            var frontendBase = _configuration["App:FrontendBaseUrl"] ?? "http://localhost:4200";
            return Redirect($"{frontendBase}/reset-password?userId={userId}&token={token}");
        }



        [HttpPost("ResendConfirmationEmail")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] string email)
        {
            var result = await _accountServices.ResendEmailConfirmation(email);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Message = "Failed to resend confirmation email",
                    Errors = result.Errors
                });
            }

            return Ok(new
            {
                Message = "Confirmation email resent successfully"
            });
        }
        //[Authorize]
        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile([FromQuery]GetProfileRequest request)
        {
            var result = await _accountServices.GetProfile(request);
            return Ok(result);
        }
        [HttpGet("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(int userId, UpdateProfileRequest request)
        {
            var result = await _accountServices.UpdateProfile(userId, request);
            return Ok(result);
        }
       

    }
}
