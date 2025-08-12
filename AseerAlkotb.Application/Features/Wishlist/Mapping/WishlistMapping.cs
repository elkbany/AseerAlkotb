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
            config.NewConfig<DomainWishlist, GetUserWishlistResponse>()
                  .Map(dest => dest.UserId, src => src.UserId)
                  .Map(dest => dest.Items, src => src.WishlistItems);

            config.NewConfig<WishlistItem, AddWishlistItemResponse>();
            config.NewConfig<DomainWishlist, ClearWishlistResponse>();
            config.NewConfig<WishlistItem, DeleteWishlistItemResponse>();

            TypeAdapterConfig<int, GetWishlistItemCountResponse>
                        .NewConfig()
                        .Map(dest => dest.Count, src => src);

            TypeAdapterConfig<bool, IsBookInWishlistResponse>
                .NewConfig()
                .Map(dest => dest.Exists, src => src);
        }
    }
}