using AseerAlkotb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Books.Requests
{
    public record FilterBooksRequest(
        string? SearchTerm,
        List<int>? CategoryIds,
        List<int>? PublisherIds,

        [property: JsonConverter(typeof(JsonStringEnumConverter))]
    BookLanguage? Language,

        [property: JsonConverter(typeof(JsonStringEnumConverter))]
    BookSortOption? SortBy,

        int PageNumber = 1,
        int PageSize = 20
    );
}
