using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Rag.Responses;

namespace AseerAlkotb.Application.Contracts
{
    public interface IWebsiteCatalogService
    {
        Task<List<BookBriefDto>> SearchAsync(string query, string? category = null, int take = 10, CancellationToken ct = default);
        Task<(BookBriefDto? book, string? authorBio)> FindByTitleOrAuthorAsync(string query, CancellationToken ct = default);
    }
}
