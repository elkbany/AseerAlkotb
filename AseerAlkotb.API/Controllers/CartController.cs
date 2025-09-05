using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.CartItem.Requests;
using AseerAlkotb.Application.Features.CartItems.Requests;
using AseerAlkotb.Application.Features.CartItems.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AseerAlkotb.API.Bases;
using Microsoft.AspNetCore.Authorization;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartServices _CartServices;
        public CartController(ICartServices cartServices)
        {
            _CartServices = cartServices;
        }

        [HttpPost("Add")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> AddItemToCart(AddItemToCartRequest request)
        {
            var result = await _CartServices.AddCartItem(request);
            return Ok(result);
            //return ApiResult(result);
        }

        [HttpPut]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> UpdateItemQuantity(UpdateItemQuantityRequest request)
        {
            var result = await _CartServices.UpdateCartItemQuantity(request);
            return Ok(result);
            //return ApiResult(result);
        }

        [HttpDelete]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> DeleteItem(DeleteItemRequest request)
        {
            var result = await _CartServices.DeleteItem(request);
            return Ok(result);
            //return ApiResult(result);
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetUserCart([FromQuery] ShowCartRequest request)
        {
            var result = await _CartServices.GetUserCart(request);
            return Ok(result);
            //return ApiResult(result);
        }

        [HttpDelete("ClearCart")]
        public async Task<IActionResult> ClearUserCart(ClearCartRequest request)
        {
            var result = await _CartServices.ClearCart(request);
            return Ok(result);
            //return ApiResult(result);
        }

    }
}
