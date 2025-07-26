using AseerAlkotb.Domain.Interfaces.ApiResponse;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.ResponseHandler
{
    public class ApiResponsePaginated<T> : ApiResponse<T> where T : ICollection
    {
        public ApiResponsePaginated()
        {

        }
        public ApiResponsePaginated(
            T data,
            int totalCount,
            int page = 1,
            int pageSize = 10, string message = null)
        {
            Data = data;
            TotalCount = totalCount;
            CurrentPage = page;
            PageSize = pageSize;
            Message = message;
        }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public int TotalCount { get; set; }
        public string Message { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    public class ApiResponse<T> : ApiResponse, IApiResponse<T>
    {
        public ApiResponse()
        {

        }
        public ApiResponse(T data, string? meta = null, string message = null)
        {
            Data = data;
            Meta = meta;
            Message = message;
        }

        public new T? Data { get; set; }
    }
    public class ApiResponse : IApiResponse
    {
        public object? Data { get; set; }
        public bool Succeeded => (int)StatusCode >= 200 && (int)StatusCode <= 290;
        public HttpStatusCode StatusCode { get; set; }
        public object? Meta { get; set; }
        public string Message { get; set; }
        public Dictionary<string, List<string>>? Errors { get; set; }

    }
}
