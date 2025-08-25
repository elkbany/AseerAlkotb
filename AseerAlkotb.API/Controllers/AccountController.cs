using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Account.Requests;
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
        private readonly IAccountServices accountServices;
        public AccountController(IAccountServices _accountServices)
        {
            accountServices = _accountServices;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await accountServices.CreateAccount(request);
            //return Ok(result);
            return Ok(new
            {
                Message = "Account created successfully",
                Data = result
            });

        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await accountServices.Login(request);
            return Ok(result);


        }
        [Authorize]
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(int userId, UpdateProfileRequest request)
        {
            var result = await accountServices.UpdateProfile(userId, request);
            return Ok(result);


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
