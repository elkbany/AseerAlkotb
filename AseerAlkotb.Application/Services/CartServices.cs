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
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class CartServices : AppService, ICartServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<User> userManager;

        public CartServices(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, IServiceProvider serviceProvider, IHostEnvironment environment) : base(serviceProvider, environment)
        {
            this.unitOfWork = unitOfWork;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public async Task<ApiResponse<ShowCartResponse>> GetUserCart(ShowCartRequest request)
        {
            await DoValidationAsync<ShowCartRequestValidation, ShowCartRequest>(request);

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<ShowCartResponse>();
            }

            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<ShowCartResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<ShowCartResponse>();
            }

            // Use current user's ID instead of request.UserId for security
            var cart = await unitOfWork.Carts.GetUserCartAsync(currentUser.Id);

            var response = new ShowCartResponse(
            Id: cart.Id,
            UserId: cart.UserId,
            Items: cart.CartItems.Select(ci => new CartItemResponse(
                BookId: ci.BookId,
                BookTitle: ci.Book.Title,
                CoverImageUrl: ci.Book.CoverImageUrl,
                UnitPrice: ci.UnitPrice,
                Quantity: ci.Quantity,
                DiscountPercentage: ci.Book.DiscountPercentage,
                DiscountedPrice: ci.UnitPrice - (ci.UnitPrice * ci.Book.DiscountPercentage / 100),
                TotalPrice: ci.TotalPrice,
                TotalDiscountedPrice: ci.Quantity * (ci.UnitPrice - (ci.UnitPrice * ci.Book.DiscountPercentage / 100))
            )),
            SumTotalPrice: cart.CartItems.Sum(ci => ci.TotalPrice),
            SumDiscountedPrice: cart.CartItems.Sum(ci => ci.Quantity * (ci.UnitPrice - (ci.UnitPrice * ci.Book.DiscountPercentage / 100))),
            TotalItemsCount: cart.CartItems.Sum(ci => ci.Quantity)

            );

            return Success(response);
        }

        public async Task<ApiResponse<AddItemToCartResponse>> AddCartItem(AddItemToCartRequest request)
        {
            AddItemToCartResponse response;
            await DoValidationAsync<AddCartItemValidation, AddItemToCartRequest>(request);

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<AddItemToCartResponse>();
            }

            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<AddItemToCartResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<AddItemToCartResponse>();
            }

            // Use current user's ID instead of request.UserId for security
            var cart = await unitOfWork.Carts.GetUserCartAsync(currentUser.Id);
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

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<DeleteItemResponse>();
            }

            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<DeleteItemResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<DeleteItemResponse>();
            }

            // Use current user's ID instead of request.UserId for security
            var cart = await unitOfWork.Carts.GetUserCartAsync(currentUser.Id);
            var item = cart.CartItems.FirstOrDefault(ci => ci.BookId == request.bookId);
            await unitOfWork.Carts.RemoveCartItemAsync(item);
            item.Book.StockQuantity += item.Quantity; ///restore Quantity in stock
            await unitOfWork.CommitAsync();
            var response = item.Adapt<DeleteItemResponse>();
            return Success(response);
        }

        public async Task<ApiResponse<UpdateItemQuantityResponse>> UpdateCartItemQuantity(UpdateItemQuantityRequest request)
        {
            await DoValidationAsync<UpdateItemQuantityValidation, UpdateItemQuantityRequest>(request);

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<UpdateItemQuantityResponse>();
            }

            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<UpdateItemQuantityResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<UpdateItemQuantityResponse>();
            }

            // Use current user's ID instead of request.UserId for security
            var cart = await unitOfWork.Carts.GetUserCartAsync(currentUser.Id);
            var item = cart.CartItems.FirstOrDefault(ci => ci.BookId == request.BookId);
            if (item.Book.StockQuantity < request.NewQuantity)
            {
                return BadRequest<UpdateItemQuantityResponse>("Stock Quantity is not enough");
            }
            else
            {
                var oldQuantity = item.Quantity;
                if (oldQuantity < request.NewQuantity)
                {

                    item.Book.StockQuantity -= request.NewQuantity - oldQuantity;
                }
                else
                {
                    item.Book.StockQuantity += oldQuantity - request.NewQuantity;
                }
                item.Quantity = request.NewQuantity;

                await unitOfWork.Carts.UpdateCartItemAsync(item);
                await unitOfWork.CommitAsync();
                var respone = item.Adapt<UpdateItemQuantityResponse>();
                return Success(respone);
            }

        }

        public async Task<ApiResponse<ClearCartResponse>> ClearCart()
        {

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<ClearCartResponse>();
            }

            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<ClearCartResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<ClearCartResponse>();
            }
            var cart = await unitOfWork.Carts.GetUserCartAsync(currentUser.Id);
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