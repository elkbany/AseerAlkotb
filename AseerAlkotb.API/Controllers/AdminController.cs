using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Roles.Requests;
using AseerAlkotb.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminServices adminServices;
        public AdminController(IAdminServices _adminServices)
        {
            this.adminServices = _adminServices;
        }


        [HttpPost("createAdminAccount")]
        public async Task<IActionResult> createAdminAccount([FromForm]CreateAdminAccountRequest request)
        {
            var result = await adminServices.createAdminAccount(request);
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

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateAdminAccount([FromRoute] int Id, [FromForm] UpdateAdminAccountRequest request)
        {
            var result = await adminServices.UpdateAdminAccount(Id, request);

            // Check if the operation was successful
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Message = result.Message,
                    Errors = result.Errors
                });
            }

            return Ok(new
            {
                Message = result.Message,
                Data = result.Data
            });
        }

        [HttpDelete("DeleteAdminAccount")]
        public async Task<IActionResult> DeleteAdminAccount(DeleteAdminAccountRequest request)
        {
            var result = await adminServices.DeleteAdminAccount(request);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Message = "can't deleted this user ",
                    Errors = result.Errors
                });
            }

            return Ok(new
            {
                Message = "Account deleted successfully.",
                Data = result.Data
            });
        }

        [HttpGet("GetAllClients")]
        public async Task<IActionResult> GetAllClients( )
        {
            var result = await adminServices.GetAllClients();

            return Ok(result);
           ;
        }

        [HttpGet("GetAllAdmins")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var result = await adminServices.GetAllAdmins();

            return Ok(result);
            ;
        }


        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole(AssignRoleRequest request)
        {
            var result = await adminServices.AssignRole(request);
            return Ok(result);
        }

        [HttpPost("RemoveRole")]
        public async Task<IActionResult> RemoveRole(RemoveRoleRequest request)
        {
            var result = await adminServices.RemoveRole(request);
            return Ok(result);
        }
    }
}
