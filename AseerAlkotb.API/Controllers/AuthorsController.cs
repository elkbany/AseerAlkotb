using AseerAlkotb.API.Bases;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Authors.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : AppControllerBase
    {
        private readonly IAuthorServices authorServices;

        public AuthorsController(IAuthorServices authorServices)
        {
            this.authorServices = authorServices;
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddAuthorRequest request)
        {
            var result = await authorServices.AddAuthorAsync(request);
            return ApiResult(result);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteAuthorRequest request)
        {
            var result =  await authorServices.DeleteAuthorAsync(request);
            return ApiResult(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetById(GetAuthorByIdRequest request)
        {
            var result = await authorServices.GetAuthorByIdAsync(request);
            return ApiResult(result);
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(GetAllAuthorsPaginatedRequest request)
        {
            var result = await authorServices.GetAllAuthorsPaginatedAsync(request);
            return ApiResult(result);
        }
        [HttpPut]
        public async Task<IActionResult> Update(UpdateAuthorRequest request)
        {
            var result = await authorServices.UpdateAuthorAsync(request);
            return ApiResult(result);
        }

    }
}
