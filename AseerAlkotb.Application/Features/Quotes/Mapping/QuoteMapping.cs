using AseerAlkotb.Application.Features.Quotes.Requests;
using AseerAlkotb.Application.Features.Quotes.Responses;
using AseerAlkotb.Domain.Entites.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Quotes.Mapping
{
    public class QuoteMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Requests
            config.NewConfig<AddQuoteRequest, Quote>()
                .Ignore(q => q.Book);

            config.NewConfig<UpdateQuoteRequest, Quote>();

            //Responses
            config.NewConfig<Quote, AddQuoteRequest>();

            config.NewConfig<Quote, UpdateQuoteResponse>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.AuthorId, src => src.AuthorId)
                .Map(dest => dest.BookId, src => src.BookId);

            config.NewConfig<Quote, DeleteQuoteResponse>();

            config.NewConfig<Quote, GetAllQuotesPaginatedResponse>()
                .Map(dest => dest.AuthorId, src => src.AuthorId)
                .Map(dest => dest.BookId, src => src.BookId)
                .Map(dest => dest.UserId, src => src.UserId);



        }
    }
}
