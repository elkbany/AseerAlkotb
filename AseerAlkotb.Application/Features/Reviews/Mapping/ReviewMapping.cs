using AseerAlkotb.Application.Features.Reviews.Requests;
using AseerAlkotb.Application.Features.Reviews.Responses;
using AseerAlkotb.Domain.Entites.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Reviews.Mapping
{
    public class ReviewMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AddReviewRequest, Review>()
                .Ignore(r => r.Book);
            config.NewConfig<Review, AddReviewResponse>();
            config.NewConfig<Review, GetReviewByIdResponse>()
                .Map(dest => dest.BookId, src => src.Book.Id)
                .Map(dest=>dest.AuthorId,src=>src.Author.Id)
                .Map(dest=>dest.UserId,src=>src.User.Id);
            config.NewConfig<Review, GetAllReviewsPaginatedResponse>()
                .Map(dest => dest.BookId, src => src.Book.Id)
                .Map(dest => dest.AuthorId, src => src.Author.Id)
                .Map(dest => dest.UserId, src => src.User.Id);
            config.NewConfig<Review, DeleteReviewResponse>();
            config.NewConfig<UpdateReviewRequest, Review>();
            config.NewConfig<Review, UpdateReviewResponse>()
                .Map(dest => dest.BookId, src => src.Book.Id)
                .Map(dest => dest.AuthorId, src => src.Author.Id)
                .Map(dest => dest.UserId, src => src.User.Id);

        }
    }
}
