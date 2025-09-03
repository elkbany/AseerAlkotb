using AseerAlkotb.API.Bases;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Wishlist.Requests;
using AseerAlkotb.Application.Features.Wishlist.Responses;
using AseerAlkotb.Application.ResponseHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : AppControllerBase
    {
        private readonly IWishlistServices wishlistServices;

        public WishlistController(IWishlistServices wishlistServices)
        {
            this.wishlistServices = wishlistServices;
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Add([FromBody] AddWishlistItemRequest request)
        {
            var result = await wishlistServices.AddToWishlistAsync(request);
            return ApiResult(result);
        }
        [HttpDelete("DeleteItem")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Delete([FromQuery] DeleteWishlistItemRequest request)
        {
            var result = await wishlistServices.RemoveFromWishlistAsync(request);
            return ApiResult(result);
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetUserWishlist([FromQuery] GetUserWishlistRequest request)
        {
            var result = await wishlistServices.GetUserWishlistAsync(request);
            return ApiResult(result);
        }

        [HttpDelete("ClearWishlist")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> ClearWishlist([FromQuery] ClearWishlistRequest request)
        {
            var result = await wishlistServices.ClearWishlistAsync(request);
            return ApiResult(result);
        }

        [HttpGet("Count")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetWishlistItemCount([FromQuery] GetWishlistItemCountRequest request)
        {
            var result = await wishlistServices.GetWishlistItemCountAsync(request);
            return ApiResult(result);
        }

        [HttpGet("IsBookInWishlist")]
        public async Task<IActionResult> IsBookInWishlist([FromQuery] IsBookInWishlistRequest request)
        {
            var result = await wishlistServices.IsBookInWishlistAsync(request);
            return ApiResult(result);
        }
    }
}
