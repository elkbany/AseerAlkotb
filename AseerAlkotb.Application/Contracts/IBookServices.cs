using AseerAlkotb.Application.Features.Books.Mapping;
using AseerAlkotb.Application.ResponseHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Contracts
{
    public interface IBookServices
    {
        public Task<ApiResponse<AddBookResponse>> AddBookAsync(AddBookRequest request);
    }
}
