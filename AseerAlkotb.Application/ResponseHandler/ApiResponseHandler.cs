using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AseerAlkotb.Application.ResponseHandler
{
    public static class ApiResponseHandler
    {
        public static ApiResponse<T> Deleted<T>(T entity)
        {
            return new ApiResponse<T>()
            {
                Data = entity,
                StatusCode = System.Net.HttpStatusCode.OK,
            };
        }
        public static ApiResponse<T> Deleted<T>(string Message = null)
        {
            return new ApiResponse<T>()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = Message == null ? "Deleted Successfully" : Message
            };
        }
        public static ApiResponse<T> Success<T>(T data, string? Message = null, object? Meta = null)
        {
            return new ApiResponse<T>()
            {
                Data = data,
                StatusCode = System.Net.HttpStatusCode.OK,
                Meta = Meta
            };
        }

        public static ApiResponse<T> Success<T>(string? Message = null)
        {
            return new ApiResponse<T>()
            {
                StatusCode = System.Net.HttpStatusCode.Created,
                Message = Message == null ? "Successfully" : Message,

            };
        }
        public static ApiResponsePaginated<T> Success<T>(T data, int totalCount, int pageNumber, int pageSize) where T : ICollection
        {
            return new ApiResponsePaginated<T>()
            {
                Data = data,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                StatusCode = System.Net.HttpStatusCode.OK,
            };
        }
        public static ApiResponse<T> UnAuthorized<T>()
        {
            return new ApiResponse<T>()
            {
                StatusCode = System.Net.HttpStatusCode.Unauthorized,
            };
        }
        public static ApiResponse<T> UnAuthorized<T>(string Message)
        {
            return new ApiResponse<T>()
            {
                StatusCode = System.Net.HttpStatusCode.Unauthorized,
                Message = Message
            };
        }
        public static ApiResponsePaginated<T> UnAutherized<T>() where T : ICollection
        {
            return new ApiResponsePaginated<T>()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
            };
        }
        public static ApiResponse<T> BadRequest<T>(string Message = null)
        {
            return new ApiResponse<T>()
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Message = Message == null ? "Bad Request" : Message
            };
        }


        public static ApiResponse<T> UnprocessableEntity<T>(T entity)
        {
            return new ApiResponse<T>()
            {
                StatusCode = System.Net.HttpStatusCode.UnprocessableEntity,
                Data = entity,
            };
        }

        public static ApiResponse<T> NotFound<T>(string Message="")
        {
            return new ApiResponse<T>()
            {
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Message = Message
            };
        }
        public static ApiResponse<T> NotFound<T>(T entity, object? Meta = null, string? Message = null)
        {
            return new ApiResponse<T>()
            {
                StatusCode = System.Net.HttpStatusCode.Created,
                Message = Message == null ? "Not Found" : Message,
                Meta = Meta,
                Data = entity
            };
        }

        public static ApiResponse<T> Created<T>(T entity, object? Meta = null)
        {
            return new ApiResponse<T>()
            {
                Data = entity,
                StatusCode = System.Net.HttpStatusCode.Created,
                Meta = Meta
            };
        }
        public static ApiResponse<T> Created<T>(string Message = null)
        {
            return new ApiResponse<T>()
            {
                StatusCode = System.Net.HttpStatusCode.Created,
                Message = Message == null ? "Created Successfully" : Message,
            };
        }
        public static ApiResponse<T> NoContent<T>()
        {
            return new ApiResponse<T>()
            {
                StatusCode = System.Net.HttpStatusCode.NoContent,
            };
        }
        public static ApiResponse<T> NoContent<T>(string Message = null)
        {
            return new ApiResponse<T>()
            {
                StatusCode = System.Net.HttpStatusCode.NoContent,
                Message = Message == null ? "Not Content" : Message
            };
        }
    }
}
