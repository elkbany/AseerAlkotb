using AseerAlkotb.API.Bases;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Publishers.Requests;
using AseerAlkotb.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : AppControllerBase
    {
        private readonly IPublisherServices _publisherServices;
        public PublishersController(IPublisherServices publisherServices)
        {
            _publisherServices = publisherServices;
        }

        [HttpPost("AddPublisher")]
        public async Task<IActionResult> Add([FromQuery] AddPublisherRequest request)
        {
            var response = await _publisherServices.AddPublisherAsync(request);
            return ApiResult(response);
        }

        [HttpGet("GetPublisherById")]
        public async Task<IActionResult> GetPublisherById([FromQuery] GetPublisherByIdRequest request)
        {
            var response = await _publisherServices.GetPublisherByIdAsync(request);
            return ApiResult(response);
        }

        [HttpGet("GetAllPublishersPaginated")]
        public async Task<IActionResult> GetAllPublishersPaginated([FromQuery] GetAllPublishersPaginatedRequest request)
        {
            var response = await _publisherServices.GetAllPublishersPaginatedAsync(request);
            return ApiResult(response);
        }

        [HttpPut("UpdatePublisher")]
        public async Task<IActionResult> Update([FromQuery] UpdatePublisherRequest request)
        {
            var response = await _publisherServices.UpdatePublisherAsync(request);
            return ApiResult(response);
        }

        [HttpDelete("DeletePublisher")]
        public async Task<IActionResult> Delete(DeletePublisherRequest request)
        {
            var response = await _publisherServices.DeletePublisherAsync(request);
            return ApiResult(response);
        }

        /////////////////////////////////follow///////////////////////////////////////////////////////
        [HttpPost("FollowPublisher")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> FollowPublisher(FollowPublisherRequest request)
        {
            var result = await _publisherServices.FollowPublisher(request);
            return ApiResult(result);
        }

        [HttpDelete("UnFollowPublisher")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> UnFollowPublisher(UnFollowPublisherRequest request)
        {
            var result = await _publisherServices.UnFollowPublisher(request);
            return ApiResult(result);
        }

        [HttpGet("GetPublisherFollowerCount")]
        public async Task<IActionResult> GetPublisherFollowerCount([FromQuery] GetPublisherFollowerCountRequest request)
        {
            var result = await _publisherServices.GetPublisherFollowerCount(request);
            return ApiResult(result);
        }

        [HttpGet("GetFollowedPublisher")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetFollowedPublisher([FromQuery] GetFollowedPublisherRequest request)
        {
            var result = await _publisherServices.GetFollowedPublisher(request);
            return ApiResult(result);
        }


        [HttpGet("GetFollowerPublisher")]
        public async Task<IActionResult> GetFollowerPublisher([FromQuery] GetFollowersPublisherRequest request)
        {
            var result = await _publisherServices.GetFollowerPublisher(request);
            return ApiResult(result);
        }
        [HttpGet("IsFollowing")]
        public async Task<IActionResult> IsFollowing([FromQuery] IsFollowingRequest request)
        {
            var result = await _publisherServices.IsFollowing(request);
            return ApiResult(result);
        }

        [HttpGet("GetAuthorRelatedToPublisher")]
        public async Task<IActionResult> GetAuthorRelatedToPublisher([FromQuery] GetAuthorRelatedToPublisherRequest request)
        {
            var result = await _publisherServices.GetAuthorRelatedToPublisher(request);
            return ApiResult(result);
        }

    }
}
