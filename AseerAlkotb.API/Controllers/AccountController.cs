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

        //[HttpPost("Register")]
        //public async Task<IActionResult> Register(RegisterRequest request)
        //{
        //    var result = await _accountServices.CreateAccount(request);
        //    //return Ok(result);
        //    return Ok(new
        //    {
        //        Message = "Account created successfully",
        //        Data = result
        //    });

        //}

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _accountServices.Register(request);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Message = "Registration failed",
                    Errors = result.Errors
                });
            }

            return Ok(new
            {
                Message = "Account created successfully. Please check your email to confirm.",
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

        //[HttpPost("Login")]
        //public async Task<IActionResult> Login(LoginRequest request)
        //{
        //    var result =await _accountServices.Login(request);

        //    return Ok(result);


        //}

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
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var result = await _accountServices.ForgotPassword(email);

            var frontendBase = _configuration["App:FrontendBaseUrl"] ?? "http://localhost:4200";

            if (!result.Succeeded)
                //return BadRequest(new { Message = "If an account with this email exists, a password reset link has been sent." });
                return Redirect($"{frontendBase}/reset-password-failed");

            //return Ok(new { Message = "If an account with this email exists, a password reset link has been sent." });
            return Redirect($"{frontendBase}/reset-password-sucess");
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
            var result = await accountServices.GetProfile(request);
            return Ok(result);
        }

    }
}
