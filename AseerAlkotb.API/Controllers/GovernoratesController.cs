using AseerAlkotb.API.Bases;
using AseerAlkotb.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GovernoratesController : AppControllerBase
    {
        private readonly IGovernorateServices governorateServices;

        public GovernoratesController(IGovernorateServices governorateServices)
        {
            this.governorateServices = governorateServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await governorateServices.GetAllGovernoratesAsync();
            return ApiResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromQuery] string name)
        {
            var result = await governorateServices.AddGovernorateAsync(name);
            return ApiResult(result);
        }
    }
}