using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Entites.Models;

namespace AseerAlkotb.Application.Features.Books.Mapping
{
    public class BookMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AddBookRequest, Book>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.DiscountedPrice)
            .Ignore(dest => dest.Author)
            .Ignore(dest => dest.Publisher)
            .Ignore(dest => dest.Categories)
            .Ignore(dest => dest.Reviews)
            .Ignore(dest => dest.OrderItems)
            .Ignore(dest => dest.CartItems)
            .Ignore(dest => dest.Wishlists)
            .Ignore(dest => dest.ViewCount)
            .Ignore(dest => dest.SalesCount);

            config.NewConfig<Book, AddBookResponse>()
                .Map(dest => dest.AuthorName, src => src.AuthorId != null ? src.Author.Name : string.Empty)
                .Map(dest => dest.PublisherName, src => src.Publisher.Name != null ? src.Publisher.Name : string.Empty)
                .Map(dest => dest.CategoryNames, src => src.Categories != null ? src.Categories.Select(c => c.Name).ToList() : new List<string>());
            
        }
    }
}
