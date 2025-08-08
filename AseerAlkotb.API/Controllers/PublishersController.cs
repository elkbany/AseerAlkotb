using AseerAlkotb.API.Bases;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Publishers.Requests;
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
    }
}
