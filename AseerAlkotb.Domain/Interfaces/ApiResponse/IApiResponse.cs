using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Interfaces.ApiResponse
{
    public interface IApiResponse<T> : IApiResponse
    {
        new T? Data { get; set; }

    }
    public interface IApiResponse
    {
        object? Data { get; set; }
        Dictionary<string, List<string>>? Errors { get; set; }
        object? Meta { get; set; }
        HttpStatusCode StatusCode { get; set; }
        bool Succeeded { get; }
    }
}
