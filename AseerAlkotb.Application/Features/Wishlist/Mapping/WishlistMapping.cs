using System.Collections.Generic;
using Mapster;
using DomainWishlist = AseerAlkotb.Domain.Entites.Models.Wishlist;
using AseerAlkotb.Application.Features.Wishlist.Responses;
using AseerAlkotb.Domain.Entites.Models;

namespace AseerAlkotb.Application.Features.Wishlist.Mapping
{
    public class WishlistMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            TypeAdapterConfig<DomainWishlist, GetUserWishlistResponse>
                .NewConfig()
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.Items, src => src.WishlistItems.Adapt<IEnumerable<WishlistItem>>());

            TypeAdapterConfig<WishlistItem, AddWishlistItemResponse>
                .NewConfig()
                .Map(dest => dest.WishlistId, src => src.WishlistId)
                .Map(dest => dest.BookId, src => src.BookId);

            TypeAdapterConfig<DomainWishlist, ClearWishlistResponse>
                .NewConfig()
                .Map(dest => dest.UserId, src => src.UserId);

            TypeAdapterConfig<WishlistItem, DeleteWishlistItemResponse>
                .NewConfig()
                .Map(dest => dest.WishlistId, src => src.WishlistId)
                .Map(dest => dest.BookId, src => src.BookId);

            TypeAdapterConfig<int, GetWishlistItemCountResponse>
                .NewConfig()
                .Map(dest => dest.Count, src => src);

            TypeAdapterConfig<bool, IsBookInWishlistResponse>
                .NewConfig()
                .Map(dest => dest.Exists, src => src);
        }
    }
}