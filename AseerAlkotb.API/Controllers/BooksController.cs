using AseerAlkotb.API.Bases;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Books.Mapping;
using AseerAlkotb.Application.Features.Books.Requests;
using AseerAlkotb.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : AppControllerBase
    {
        private readonly IBookServices _bookServices;

        public BooksController(IBookServices bookServices)
        {
            _bookServices = bookServices;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromQuery] AddBookRequest request)
        {
            var result = await _bookServices.AddBookAsync(request);
            return ApiResult(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromQuery] UpdateBookRequest request)
        {
            var result = await _bookServices.UpdateBookAsync(request);
            return ApiResult(result);

        }

        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteBookRequest request)
        {
            var result = await _bookServices.DeleteBookAsync(request);
            return ApiResult(result);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] GetBookByIdRequest request)
        {
            var result = await _bookServices.GetBookByIdAsync(request);
            return ApiResult(result);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]GetAllBooksPaginatedRequest request)
        {
            var result = await _bookServices.GetAllBooksPaginatedAsync(request);
            return ApiResult(result);
        }

        [HttpGet("Filter")]
        public async Task<IActionResult> FilterBooksAsync([FromQuery] FilterBooksRequest request)
        {
            var result = await _bookServices.FilterBooksAsync(request);
            return Ok(result);
        }

    }
}
