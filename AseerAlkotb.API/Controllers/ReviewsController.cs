using AseerAlkotb.API.Bases;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Reviews.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : AppControllerBase
    {
        private readonly IReviewServices reviewServices;

        public ReviewsController(IReviewServices reviewServices)
        {
            this.reviewServices = reviewServices;
        }
        [HttpPost]
        public async Task<IActionResult> AddReviewAsync(AddReviewRequest request)
        {
            var response = await reviewServices.AddReviewAsync(request);
            return ApiResult(response);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateReviewAsync(UpdateReviewRequest request)
        {
            var response = await reviewServices.UpdateReviewAsync(request);
            return ApiResult(response);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteReviewAsync(DeleteReviewRequest request)
        {
            var response = await reviewServices.DeleteReviewAsync(request);
            return ApiResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetReviewByIdAsync(GetReviewByIdRequest request)
        {
            var response = await reviewServices.GetReviewByIdAsync(request);
            return ApiResult(response);
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllReviewsAsync([FromQuery] GetAllReviewsPaginatedRequest request)
        {
            var response = await reviewServices.GetAllReviewsAsync(request);
            return ApiResult(response);
        }
        [HttpPost("Like")]
        public async Task<IActionResult> LikeReviewAsync(LikeReviewRequest request)
        {
            var response = await reviewServices.LikeReviewAsync(request);
            return ApiResult(response);
        }

    }
}
