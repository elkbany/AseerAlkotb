using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Wishlist.Requests;
using AseerAlkotb.Application.Features.Wishlist.Responses;
using AseerAlkotb.Application.Features.Wishlist.Validators;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class WishlistServices : AppService, IWishlistServices
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<User> userManager;
        private readonly IUnitOfWork unitOfWork;

        public WishlistServices(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IHostEnvironment environment)
            : base(serviceProvider, environment)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<GetUserWishlistResponse>> GetUserWishlistAsync(GetUserWishlistRequest request)
        {
            await DoValidationAsync<GetUserWishlistValidation, GetUserWishlistRequest>(request);

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<GetUserWishlistResponse>();
            }

            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<GetUserWishlistResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<GetUserWishlistResponse>();
            }

            // Use current user's ID instead of request.UserId for security
            var wishlist = await unitOfWork.Wishlists.GetUserWishlistAsync(currentUser.Id);
            var wishlistItems = wishlist?.WishlistItems ?? new List<WishlistItem>();
            var response = new GetUserWishlistResponse(
                currentUser.Id,
                wishlistItems.Select(wi => new WishlistItemResponse(
                    wi.BookId,
                    wi.Book.Title,
                    wi.Book.Description,
                    wi.Book.Price,
                    wi.Book.Author?.Name ?? string.Empty,
                    wi.Book.CoverImageUrl
                ))
            );
            return Success(response);
        }

        public async Task<ApiResponse<AddWishlistItemResponse>> AddToWishlistAsync(AddWishlistItemRequest request)
        {
            await DoValidationAsync<AddWishlistItemValidation, AddWishlistItemRequest>(request);

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<AddWishlistItemResponse>();
            }

            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<AddWishlistItemResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<AddWishlistItemResponse>();
            }

            var book = await unitOfWork.Books.GetByIdAsync(request.BookId);
            if (book == null)
            {
                return NotFound<AddWishlistItemResponse>($"{_stringLocalizer["Book"]} {_stringLocalizer["NotFound"]}");
            }

            // Use current user's ID instead of request.UserId for security
            var isAlreadyInWishlist = await unitOfWork.Wishlists.IsBookInWishlistAsync(currentUser.Id, request.BookId);
            if (isAlreadyInWishlist)
            {
                return BadRequest<AddWishlistItemResponse>($"{_stringLocalizer["Book"]} {_stringLocalizer["AlreadyInWishlist"]}");
            }

            var wishlist = await unitOfWork.Wishlists.GetUserWishlistAsync(currentUser.Id);
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = currentUser.Id,
                    WishlistItems = new List<WishlistItem>(),
                    CreatedAt = DateTime.UtcNow
                };
                await unitOfWork.Wishlists.InsertAsync(wishlist);
                await unitOfWork.CommitAsync();
            }

            var wishlistItem = new WishlistItem
            {
                BookId = request.BookId,
                WishlistId = wishlist.Id,
            };

            await unitOfWork.Wishlists.AddWishlistItemAsync(wishlistItem);
            await unitOfWork.CommitAsync();

            var response = wishlistItem.Adapt<AddWishlistItemResponse>();
            return Success(response);
        }

        public async Task<ApiResponse<DeleteWishlistItemResponse>> RemoveFromWishlistAsync(DeleteWishlistItemRequest request)
        {
            await DoValidationAsync<DeleteWishlistItemValidation, DeleteWishlistItemRequest>(request);

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<DeleteWishlistItemResponse>();
            }

            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<DeleteWishlistItemResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<DeleteWishlistItemResponse>();
            }

            // Use current user's ID instead of request.UserId for security
            var wishlist = await unitOfWork.Wishlists.GetUserWishlistAsync(currentUser.Id);
            if (wishlist == null)
            {
                return NotFound<DeleteWishlistItemResponse>($"{_stringLocalizer["Wishlist"]} {_stringLocalizer["NotFound"]}");
            }

            var wishlistItem = wishlist.WishlistItems?.FirstOrDefault(wi => wi.BookId == request.BookId);
            if (wishlistItem == null)
            {
                return NotFound<DeleteWishlistItemResponse>($"{_stringLocalizer["Book"]} {_stringLocalizer["NotFoundInWishlist"]}");
            }

            await unitOfWork.Wishlists.RemoveWishlistItemAsync(wishlistItem);
            await unitOfWork.CommitAsync();

            var response = wishlistItem.Adapt<DeleteWishlistItemResponse>();
            return Success(response);
        }

        public async Task<ApiResponse<GetWishlistItemCountResponse>> GetWishlistItemCountAsync(GetWishlistItemCountRequest request)
        {
            await DoValidationAsync<GetWishlistItemCountValidation, GetWishlistItemCountRequest>(request);

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<GetWishlistItemCountResponse>();
            }

            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<GetWishlistItemCountResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<GetWishlistItemCountResponse>();
            }

            // Use current user's ID instead of request.UserId for security
            var count = await unitOfWork.Wishlists.GetWishlistItemCountAsync(currentUser.Id);
            var response = new GetWishlistItemCountResponse(count);
            return Success(response);
        }

        public async Task<ApiResponse<IsBookInWishlistResponse>> IsBookInWishlistAsync(IsBookInWishlistRequest request)
        {
            await DoValidationAsync<IsBookInWishlistValidation, IsBookInWishlistRequest>(request);

            // This method can remain public or you can add authorization as needed
            // For now, keeping it public for general checking purposes
            var exists = await unitOfWork.Wishlists.IsBookInWishlistAsync(request.UserId, request.BookId);
            var response = new IsBookInWishlistResponse(exists);
            return Success(response);
        }

        public async Task<ApiResponse<ClearWishlistResponse>> ClearWishlistAsync(ClearWishlistRequest request)
        {
            await DoValidationAsync<ClearWishlistValidation, ClearWishlistRequest>(request);

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<ClearWishlistResponse>();
            }

            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<ClearWishlistResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<ClearWishlistResponse>();
            }

            // Use current user's ID instead of request.UserId for security
            var wishlist = await unitOfWork.Wishlists.GetUserWishlistAsync(currentUser.Id);
            if (wishlist == null || !wishlist.WishlistItems.Any())
            {
                return BadRequest<ClearWishlistResponse>($"{_stringLocalizer["Wishlist"]} {_stringLocalizer["EmptyOrNotFound"]}");
            }

            await unitOfWork.Wishlists.ClearWishlistAsync(currentUser.Id);
            await unitOfWork.CommitAsync();

            var response = wishlist.Adapt<ClearWishlistResponse>();
            return Success(response);
        }
    }
}