using AseerAlkotb.API.Bases;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Rag.Requests;
using AseerAlkotb.Application.Features.Rag.Responses;
using AseerAlkotb.Application.ResponseHandler;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RagController : AppControllerBase
    {
        private readonly IRagService _rag;
        private readonly IEmbeddingService _emb;

        public RagController(IRagService rag, IEmbeddingService emb)
        {
            _rag = rag; _emb = emb;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] RagAskRequest request)
        {
            var res = await _rag.AskAsync(request);
            return ApiResult(res);
        }

        [HttpGet("book-availability")]
        public async Task<IActionResult> Availability([FromQuery] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest(ApiResponseHandler.BadRequest<string>("bookTitle مطلوب"));
            var res = await _rag.GetBookAvailabilityAsync(title);
            return ApiResult(res);
        }

        [HttpGet("author-books")]
        public async Task<IActionResult> AuthorBooks([FromQuery] string author)
        {
            var res = await _rag.GetAuthorBooksAsync(author);
            return ApiResult(res);
        }

        [HttpGet("category-books")]
        public async Task<IActionResult> CategoryBooks([FromQuery] string category)
        {
            var res = await _rag.GetCategoryBooksAsync(category);
            return ApiResult(res);
        }

        [HttpGet("recommendations")]
        public async Task<IActionResult> Recs([FromQuery] string q)
        {
            var res = await _rag.GetRecommendationsAsync(q);
            return ApiResult(res);
        }


        [HttpGet("smart-search")]
        public async Task<IActionResult> SmartSearch([FromQuery] RagSmartSearchRequest request)
        {
            var query = string.IsNullOrWhiteSpace(request.Query) ? "" : request.Query.Trim();
            var k = request.TopK > 0 ? request.TopK : 10;

            var hits = await _emb.SearchSimilarBooksAsync(query, k);

            var data = hits
                .Where(h => h.Book != null)
                .Select(h => new BookBriefDto(
                    Id: h.Book!.Id,
                    Title: h.Book!.Title,
                    AuthorName: h.Book!.Author?.Name,
                    Price: h.Book!.Price,
                    DiscountedPrice: h.Book!.DiscountedPrice,
                    CoverImageUrl: h.Book!.CoverImageUrl,
                    Description: h.Book!.Description
                ))
                .ToList();

            return Ok(ApiResponseHandler.Success(data));
        }


    }
}
