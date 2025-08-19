using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Authors.Validators;
using AseerAlkotb.Application.Features.CartItem.Requests;
using AseerAlkotb.Application.Features.CartItem.Responses;
using AseerAlkotb.Application.Features.CartItems.Requests;
using AseerAlkotb.Application.Features.CartItems.Responses;
using AseerAlkotb.Application.Features.CartItems.Validation;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class CartServices : AppService,ICartServices
    {
        private readonly IUnitOfWork unitOfWork;

        public CartServices(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IHostEnvironment environment) : base(serviceProvider, environment)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<ShowCartResponse>> GetUserCart(ShowCartRequest request)
        {
            await DoValidationAsync<ShowCartRequestValidation, ShowCartRequest>(request);
            var cart = await unitOfWork.Carts.GetUserCartAsync(request.UserId);
            //var response=cart.Adapt<ShowCartResponse>();
            var response = new ShowCartResponse(
            Id: cart.Id,
            UserId: cart.UserId,
            Items: cart.CartItems.Select(ci => new CartItemResponse(
                BookId: ci.BookId,
                BookTitle: ci.Book.Title,
                UnitPrice: ci.UnitPrice,
                Quantity: ci.Quantity,
                TotalPrice: ci.TotalPrice
            )) ,
            SumTotalPrice: cart.CartItems.Sum(ci => ci.TotalPrice) 
            );

            return Success(response);
        }
        public async Task<ApiResponse<AddItemToCartResponse>> AddCartItem(AddItemToCartRequest request)
        {
            AddItemToCartResponse response;
            await DoValidationAsync<AddCartItemValidation, AddItemToCartRequest>(request);
            var cart = await unitOfWork.Carts.GetUserCartAsync(request.UserId);
            var item = await unitOfWork.Books.GetByIdAsync(request.BookId);
            if (item.StockQuantity < 1)
            {
                return NotFound<AddItemToCartResponse>("Item is out of stock");
            }
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.BookId == request.BookId);
            if (existingItem != null)
            {
                existingItem.Quantity += 1;
                item.StockQuantity -= 1;
                await unitOfWork.Carts.UpdateCartItemAsync(existingItem);
                await unitOfWork.CommitAsync();
                response = existingItem.Adapt<AddItemToCartResponse>();
            }
            else
            {
                var newItem = new CartItem
                {
                    BookId = item.Id,
                    Quantity = 1,
                    UnitPrice = item.Price,
                    CartId = cart.Id
                };
                item.StockQuantity -= 1;
                await unitOfWork.Carts.AddCartItemAsync(newItem);
                await unitOfWork.CommitAsync();
                response = newItem.Adapt<AddItemToCartResponse>();
            }
            return Success(response);
        }

        public async Task<ApiResponse<DeleteItemResponse>> DeleteItem(DeleteItemRequest request)
        {
            await DoValidationAsync<DeleteItemValidation, DeleteItemRequest>(request);
            var cart = await unitOfWork.Carts.GetUserCartAsync(request.UserId);
            var item= cart.CartItems.FirstOrDefault(ci=>ci.BookId==request.bookId);
            await unitOfWork.Carts.RemoveCartItemAsync(item);
            item.Book.StockQuantity += item.Quantity; ///restor Quantity in stock
            await unitOfWork.CommitAsync();
            var response = item.Adapt<DeleteItemResponse>();
            return Success(response);
        }

        public async Task<ApiResponse<UpdateItemQuantityResponse>> UpdateCartItemQuantity(UpdateItemQuantityRequest request)
        {
            await DoValidationAsync<UpdateItemQuantityValidation, UpdateItemQuantityRequest>(request);
            var cart = await unitOfWork.Carts.GetUserCartAsync(request.UserId);
            var item= cart.CartItems.FirstOrDefault(ci=>ci.BookId==request.BookId);
            if (item.Book.StockQuantity < request.NewQuantity)
            {
                return BadRequest<UpdateItemQuantityResponse>("Stock Quantity is not enough");
            }
            else
            {
                var oldQuantity = item.Quantity;
                if (oldQuantity < request.NewQuantity) {

                    item.Book.StockQuantity -= request.NewQuantity - oldQuantity;
                }
                else
                {
                    item.Book.StockQuantity += oldQuantity-request.NewQuantity ;
                }
                item.Quantity = request.NewQuantity;
               
                await unitOfWork.Carts.UpdateCartItemAsync(item);
                await unitOfWork.CommitAsync();
                var respone = item.Adapt<UpdateItemQuantityResponse>();
                return Success(respone);
            }
            
        }

        public async Task<ApiResponse<ClearCartResponse>> ClearCart(ClearCartRequest request)
        {
            await DoValidationAsync<ClearCartRequestValidation, ClearCartRequest>(request);
            var cart = await unitOfWork.Carts.GetUserCartAsync(request.UserId);
            foreach (var item in cart.CartItems.ToList())
            {
                await unitOfWork.Carts.RemoveCartItemAsync(item);
                item.Book.StockQuantity += item.Quantity;
            }
            await unitOfWork.CommitAsync();
            var response = cart.Adapt<ClearCartResponse>();
            return Success(response);
        }
    }
}
