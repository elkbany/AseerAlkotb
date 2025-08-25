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
