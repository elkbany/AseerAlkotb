using AseerAlkotb.API.Bases;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Authors.Requests;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Add([FromQuery]AddAuthorRequest request)
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
        public async Task<IActionResult> GetById([FromQuery]GetAuthorByIdRequest request)
        {
            var result = await authorServices.GetAuthorByIdAsync(request);
            return ApiResult(result);
        }
        //[Authorize]/////////for test
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]GetAllAuthorsPaginatedRequest request)
        {
            var result = await authorServices.GetAllAuthorsPaginatedAsync(request);
            return ApiResult(result);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromQuery]UpdateAuthorRequest request)
        {
            var result = await authorServices.UpdateAuthorAsync(request);
            return ApiResult(result);
        }

        /////////////////////////////////follow///////////////////////////////////////////////////////
        [HttpPost("FollowAuther")]
        public async Task<IActionResult> FollowAuther(FollowAutherRequest request)
        {
            var result=await authorServices.FollowAuther(request);
            return ApiResult(result);
        }

        [HttpDelete("UnFollowAuthor")]
        public async Task<IActionResult> UnFollowAuthor(UnFollowAuthorRequest request)
        {
            var result = await authorServices.UnFollowAuthor(request);
            return ApiResult(result);
        }

        [HttpGet("GetAutherFollowerCount")]
        public async Task<IActionResult> GetAutherFollowerCount([FromQuery]GetAutherFollowerCountRequest request)
        {
            var result = await authorServices.GetAutherFollowerCount(request);
            return ApiResult(result);
        }

        [HttpGet("GetFollowedAuther")]
        public async Task<IActionResult> GetFollowedAuther([FromQuery] GetFollowedAuthorRequest request)
        {
            var result = await authorServices.GetFollowedAuther(request);
            return ApiResult(result);
        }


        [HttpGet("GetFollowerAuther")]
        public async Task<IActionResult> GetFollowerAuther([FromQuery] GetFollowersAuthorRequest request)
        {
            var result = await authorServices.GetFollowerAuther(request);
            return ApiResult(result);
        }



    }
}
