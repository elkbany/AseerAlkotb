using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Rag.Responses;
using AseerAlkotb.Domain.Entites.Models;
using Mapster;

namespace AseerAlkotb.Application.Features.Rag.Mapping
{
    public class RagMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Book, BookBriefDto>()
                .Map(d => d.Id, s => s.Id)
                .Map(d => d.Title, s => s.Title) // لو عندك Name أو Title
                .Map(d => d.AuthorName, s => s.Author != null ? s.Author.Name : null)
                .Map(d => d.Price, s => s.Price)
                .Map(d => d.DiscountedPrice, s => s.DiscountedPrice)
                .Map(d => d.CoverImageUrl, s => s.CoverImageUrl);
        }
    }
}
