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
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class WishlistServices : AppService, IWishlistServices
    {
        private readonly IUnitOfWork unitOfWork;

        public WishlistServices(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IHostEnvironment environment)
            : base(serviceProvider, environment)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<GetUserWishlistResponse>> GetUserWishlistAsync(GetUserWishlistRequest request)
        {
            await DoValidationAsync<GetUserWishlistValidation, GetUserWishlistRequest>(request);
            var wishlist = await unitOfWork.Wishlists.GetUserWishlistAsync(request.UserId);
            var wishlistItems = wishlist?.WishlistItems ?? new List<WishlistItem>();
            var response = new GetUserWishlistResponse(
                request.UserId,
                wishlistItems.Select(wi => new WishlistItemResponse(
                    wi.BookId,
                    wi.Book.Title,
                    wi.Book.Description,
                    wi.Book.Price,
                    wi.Book.Author?.Name ?? string.Empty,
                    wi.Book.CoverImageUrl
                ))
            ); return Success(response);
        }

        public async Task<ApiResponse<AddWishlistItemResponse>> AddToWishlistAsync(AddWishlistItemRequest request)
        {
            await DoValidationAsync<AddWishlistItemValidation, AddWishlistItemRequest>(request);
            var book = await unitOfWork.Books.GetByIdAsync(request.BookId);
            if (book == null)
            {
                return NotFound<AddWishlistItemResponse>("Book not found");
            }

            var isAlreadyInWishlist = await unitOfWork.Wishlists.IsBookInWishlistAsync(request.UserId, request.BookId);
            if (isAlreadyInWishlist)
            {
                return BadRequest<AddWishlistItemResponse>("Book is already in wishlist");
            }

            var wishlist = await unitOfWork.Wishlists.GetUserWishlistAsync(request.UserId);
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = request.UserId,
                    WishlistItems = new List<WishlistItem>()
                };
                await unitOfWork.Wishlists.InsertAsync(wishlist);
                await unitOfWork.CommitAsync();
            }

            var wishlistItem = new WishlistItem
            {
                BookId = request.BookId,
                WishlistId = wishlist.Id
            };

            await unitOfWork.Wishlists.AddWishlistItemAsync(wishlistItem);
            await unitOfWork.CommitAsync();

            var response = wishlistItem.Adapt<AddWishlistItemResponse>();
            return Success(response);
        }

        public async Task<ApiResponse<DeleteWishlistItemResponse>> RemoveFromWishlistAsync(DeleteWishlistItemRequest request)
        {
            await DoValidationAsync<DeleteWishlistItemValidation, DeleteWishlistItemRequest>(request);
            var wishlist = await unitOfWork.Wishlists.GetUserWishlistAsync(request.UserId);
            if (wishlist == null)
            {
                return NotFound<DeleteWishlistItemResponse>("Wishlist not found");
            }

            var wishlistItem = wishlist.WishlistItems?.FirstOrDefault(wi => wi.BookId == request.BookId);
            if (wishlistItem == null)
            {
                return NotFound<DeleteWishlistItemResponse>("Book not found in wishlist");
            }

            await unitOfWork.Wishlists.RemoveWishlistItemAsync(wishlistItem);
            await unitOfWork.CommitAsync();

            var response = wishlistItem.Adapt<DeleteWishlistItemResponse>();
            return Success(response);
        }

        public async Task<ApiResponse<GetWishlistItemCountResponse>> GetWishlistItemCountAsync(GetWishlistItemCountRequest request)
        {
            await DoValidationAsync<GetWishlistItemCountValidation, GetWishlistItemCountRequest>(request);
            var count = await unitOfWork.Wishlists.GetWishlistItemCountAsync(request.UserId);
            var response = new GetWishlistItemCountResponse(count);
            return Success(response);
        }

        public async Task<ApiResponse<IsBookInWishlistResponse>> IsBookInWishlistAsync(IsBookInWishlistRequest request)
        {
            await DoValidationAsync<IsBookInWishlistValidation, IsBookInWishlistRequest>(request);
            var exists = await unitOfWork.Wishlists.IsBookInWishlistAsync(request.UserId, request.BookId);
            var response = new IsBookInWishlistResponse(exists);
            return Success(response);
        }

        public async Task<ApiResponse<ClearWishlistResponse>> ClearWishlistAsync(ClearWishlistRequest request)
        {
            await DoValidationAsync<ClearWishlistValidation, ClearWishlistRequest>(request);
            var wishlist = await unitOfWork.Wishlists.GetUserWishlistAsync(request.UserId);
            if (wishlist == null || !wishlist.WishlistItems.Any())
            {
                return BadRequest<ClearWishlistResponse>("Wishlist is already empty or not found");
            }

            await unitOfWork.Wishlists.ClearWishlistAsync(request.UserId);
            await unitOfWork.CommitAsync();

            var response = wishlist.Adapt<ClearWishlistResponse>();
            return Success(response);
        }
    }
}
