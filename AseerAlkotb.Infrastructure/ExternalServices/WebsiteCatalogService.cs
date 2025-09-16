using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Rag.Responses;

namespace AseerAlkotb.Infrastructure.ExternalServices
{
    public class WebsiteCatalogService : IWebsiteCatalogService
    {
        private readonly HttpClient _http;
        private readonly string _base;

        public WebsiteCatalogService(IHttpClientFactory http, IConfiguration cfg)
        {
            _http = http.CreateClient("website");
            _base = cfg["Website:BaseUrl"]?.TrimEnd('/') ?? "https://aseeralkotb.com";
        }

        public async Task<List<BookBriefDto>> SearchAsync(string query, string? category = null, int take = 10, CancellationToken ct = default)
        {
            var url = string.IsNullOrWhiteSpace(category)
                ? $"{_base}/api/search?query={Uri.EscapeDataString(query)}&take={take}"
                : $"{_base}/api/search?query={Uri.EscapeDataString(query)}&category={Uri.EscapeDataString(category)}&take={take}";

            try
            {
                var items = await _http.GetFromJsonAsync<List<BookBriefDto>>(url, ct);
                return items ?? new List<BookBriefDto>();
            }
            catch
            {
                return new List<BookBriefDto>();
            }
        }

        public async Task<(BookBriefDto? book, string? authorBio)> FindByTitleOrAuthorAsync(string query, CancellationToken ct = default)
        {
            var url = $"{_base}/api/lookup?q={Uri.EscapeDataString(query)}";
            try
            {
                var dto = await _http.GetFromJsonAsync<LookupDto>(url, ct);
                return (dto?.Book, dto?.AuthorBio);
            }
            catch
            {
                return (null, null);
            }
        }

        private sealed class LookupDto
        {
            public BookBriefDto? Book { get; set; }
            public string? AuthorBio { get; set; }
        }
    }
}
