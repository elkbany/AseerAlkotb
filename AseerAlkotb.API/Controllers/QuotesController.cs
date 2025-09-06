using AseerAlkotb.API.Bases;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Quotes.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : AppControllerBase
    {
        private readonly IQuoteService quoteService;

        public QuotesController(IQuoteService quoteService)
        {
            this.quoteService = quoteService;
        }

        [HttpPost("AddQuote")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> AddQuote([FromBody] AddQuoteRequest request)
        {
            var response = await quoteService.AddQuoteAsync(request);
            return ApiResult(response);
        }

        [HttpGet("GetQuoteById")]
        public async Task<IActionResult> GetQuoteById([FromQuery] GetQuoteByIdRequest request)
        {
            var response = await quoteService.GetQuoteByIdAsync(request);
            return ApiResult(response);
        }

        [HttpGet("GetAllQuotes")]
        public async Task<IActionResult> GetAllQuotes([FromQuery] GetAllQuotesPaginatedRequest request)
        {
            var response = await quoteService.GetAllQuotesAsync(request);
            return ApiResult(response);
        }

        [HttpPut("UpdateQuote")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> UpdateQuote(UpdateQuoteRequest request)
        {
            var response = await quoteService.UpdateQuoteAsync(request);
            return ApiResult(response);
        }

        [HttpDelete("DeleteQuote/{id}")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> DeleteQuote([FromRoute] DeleteQuoteRequest request)
        {
            var response = await quoteService.DeleteQuoteAsync(request);
            return ApiResult(response);
        }
    }
}
