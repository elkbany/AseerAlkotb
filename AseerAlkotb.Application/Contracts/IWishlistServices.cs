using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Wishlist.Requests;
using AseerAlkotb.Application.Features.Wishlist.Responses;
using AseerAlkotb.Application.ResponseHandler;

namespace AseerAlkotb.Application.Contracts
{
    public interface IWishlistServices
    {
        Task<ApiResponse<AddWishlistItemResponse>> AddToWishlistAsync(AddWishlistItemRequest request);
        Task<ApiResponse<DeleteWishlistItemResponse>> RemoveFromWishlistAsync(DeleteWishlistItemRequest request);
        Task<ApiResponse<GetUserWishlistResponse>> GetUserWishlistAsync();
        Task<ApiResponse<GetWishlistItemCountResponse>> GetWishlistItemCountAsync();
        Task<ApiResponse<IsBookInWishlistResponse>> IsBookInWishlistAsync(IsBookInWishlistRequest request);
        Task<ApiResponse<ClearWishlistResponse>> ClearWishlistAsync();
        public Task<ApiResponsePaginated<List<GetWishlistItemsResponse>>> GetwishlistItemsAsync(GetWishlistItemsRequest request);

    }
}
