using AseerAlkotb.API.Bases;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Categories.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : AppControllerBase
    {
        private readonly ICategoryServices categoryServices;

        public CategoriesController(ICategoryServices categoryServices)
        {
            this.categoryServices = categoryServices;
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromQuery] AddCategoryRequest request)
        {
            var result = await categoryServices.AddCategoryAsync(request);
            return ApiResult(result);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteCategoryRequest request)
        {
            var result = await categoryServices.DeleteCategoryAsync(request);
            return ApiResult(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] GetCategoryByIdRequest request)
        {
            var result = await categoryServices.GetCategoryByIdAsync(request);
            return ApiResult(result);
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllCategoriesPaginatedRequest request)
        {
            var result = await categoryServices.GetAllCategoriesPaginatedAsync(request);
            return ApiResult(result);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromQuery] UpdateCategoryRequest request)
        {
            var result = await categoryServices.UpdateCategoryAsync(request);
            return ApiResult(result);
        }

    }
}