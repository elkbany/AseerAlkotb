using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Application.Features.Books.Requests;
using AseerAlkotb.Application.Features.Books.Responses;

namespace AseerAlkotb.Application.Features.Books.Mapping
{
    public class BookMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            #region Add Book Mapping
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
            #endregion

            #region Update Book Mapping
            config.NewConfig<UpdateBookRequest, Book>()
                .Ignore(dest => dest.CoverImageUrl)
                .Ignore(dest => dest.Categories);

            config.NewConfig<Book, UpdateBookResponse>()
                    .Map(dest => dest.CategoryIds, src => src.Categories.Select(c => c.Id).ToList());



            #endregion

            #region Delete Book Mapping
            config.NewConfig<DeleteBookRequest, Book>();
            #endregion

            #region Get Book By Id Mapping
            TypeAdapterConfig<Book, GetBookByIdResponse>.NewConfig()
                .Map(dest => dest.CategoryIds, src => src.Categories.Select(c => c.Id).ToList())
                .Map(dest => dest.CategoryNames, src => src.Categories.Select(c => c.Name).ToList());
            #endregion

            #region Get All Books Mapping
            config.NewConfig<Book, GetAllBooksPaginatedResponse>()
                .Map(dest => dest.AuthorName, src => src.AuthorId != null ? src.Author.Name : string.Empty)
                .Map(dest => dest.PublisherName, src => src.Publisher.Name != null ? src.Publisher.Name : string.Empty)
                .Map(dest => dest.CategoryNames, src => src.Categories != null ? src.Categories.Select(c => c.Name).ToList() : new List<string>());
            #endregion
        }
    }
}
