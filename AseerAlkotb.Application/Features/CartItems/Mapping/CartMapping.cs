using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Authors.Responses;
using AseerAlkotb.Application.Features.CartItem.Responses;
using AseerAlkotb.Application.Features.CartItems.Responses;
using AseerAlkotb.Domain.Entites.Models;
using Mapster;
using DomainCartItem = AseerAlkotb.Domain.Entites.Models.CartItem;

namespace AseerAlkotb.Application.Features.CartItem.Mapping
{
    public class CartMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            //TypeAdapterConfig<Cart, ShowCartResponse>
            //    .NewConfig()
            //    .Map(dest => dest.Id, src => src.Id)
            //    .Map(dest => dest.UserId, src => src.UserId)
            //    .Map(dest => dest.Items, src => src.CartItems.Adapt<IEnumerable<CartItemResponse>>())
            //    .Map(dest => dest.SumTotalPrice, src => src.CartItems.Sum(item => item.TotalPrice));

            //TypeAdapterConfig<DomainCartItem, CartItemResponse>
            //    .NewConfig()
            //    //.Map(dest => dest.Id, src => src.Id)
            //    .Map(dest => dest.BookId, src => src.BookId)
            //    .Map(dest => dest.BookTitle, src => src.Book.Title)
            //    .Map(dest => dest.UnitPrice, src => src.UnitPrice)
            //    .Map(dest => dest.Quantity, src => src.Quantity)
            //    .Map(dest => dest.TotalPrice, src => src.TotalPrice);

            #region Add
            TypeAdapterConfig<Cart, ClearCartResponse>
             .NewConfig()
             .Map(dest => dest.CartId, src => src.Id);
             
            #endregion

            TypeAdapterConfig<DomainCartItem, DeleteItemResponse>
                .NewConfig()
                .Map(dest => dest.BookId, src => src.BookId)
                .Map(dest => dest.CartId, src => src.CartId);


            TypeAdapterConfig<DomainCartItem, UpdateItemQuantityResponse>
                .NewConfig()
                .Map(dest => dest.BookId, src => src.BookId)
                .Map(dest => dest.CartId, src => src.CartId);


            TypeAdapterConfig<DomainCartItem, AddItemToCartResponse>
                .NewConfig()
                .Map(dest => dest.bookId, src => src.BookId)
                .Map(dest => dest.cartId, src => src.CartId);
        }
    }
}
