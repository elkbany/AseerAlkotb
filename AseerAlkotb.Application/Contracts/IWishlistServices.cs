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
        Task<ApiResponse<GetUserWishlistResponse>> GetUserWishlistAsync(GetUserWishlistRequest request);
        Task<ApiResponse<GetWishlistItemCountResponse>> GetWishlistItemCountAsync(GetWishlistItemCountRequest request);
        Task<ApiResponse<IsBookInWishlistResponse>> IsBookInWishlistAsync(IsBookInWishlistRequest request);
        Task<ApiResponse<ClearWishlistResponse>> ClearWishlistAsync(ClearWishlistRequest request);
        public Task<ApiResponsePaginated<List<GetWishlistItemsResponse>>> GetwishlistItemsAsync(GetWishlistItemsRequest request);

    }
}
