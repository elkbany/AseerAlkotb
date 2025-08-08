using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.CartItem.Requests;
using AseerAlkotb.Application.Features.CartItem.Responses;
using AseerAlkotb.Application.Features.CartItems.Requests;
using AseerAlkotb.Application.Features.CartItems.Responses;
using AseerAlkotb.Application.Features.CartItems.Validation;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;

namespace AseerAlkotb.Application.Contracts
{
    public interface ICartServices
    {
        Task<ApiResponse<ShowCartResponse>> GetUserCart(ShowCartRequest request);

        Task<ApiResponse<AddItemToCartResponse>> AddCartItem(AddItemToCartRequest request);


        Task<ApiResponse<DeleteItemResponse>> DeleteItem(DeleteItemRequest request);


        Task<ApiResponse<UpdateItemQuantityResponse>> UpdateCartItemQuantity(UpdateItemQuantityRequest request);

        Task<ApiResponse<ClearCartResponse>> ClearCart(ClearCartRequest request);
    }
}
