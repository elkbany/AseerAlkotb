using AseerAlkotb.API.Bases;
using AseerAlkotb.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : AppControllerBase
    {
        private readonly ICityServices cityServices;

        public CitiesController(ICityServices cityServices)
        {
            this.cityServices = cityServices;
        }

        [HttpGet("governorate/{governorateId}")]
        public async Task<IActionResult> GetByGovernorate([FromRoute] int governorateId)
        {
            var result = await cityServices.GetCitiesByGovernorateAsync(governorateId);
            return ApiResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromQuery] string name, [FromQuery] int governorateId)
        {
            var result = await cityServices.AddCityAsync(name, governorateId);
            return ApiResult(result);
        }
    }
}