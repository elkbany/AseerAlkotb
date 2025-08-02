using AseerAlkotb.Application.Features.Books.Mapping;
using AseerAlkotb.Application.Features.Books.Requests;
using AseerAlkotb.Application.Features.Books.Responses;
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
        public Task<ApiResponse<UpdateBookResponse>> UpdateBookAsync(UpdateBookRequest request);
        public Task<ApiResponse<DeleteBookResponse>> DeleteBookAsync(DeleteBookRequest request);
        public Task<ApiResponse<GetBookByIdResponse>> GetBookByIdAsync(GetBookByIdRequest request);
        public Task<ApiResponsePaginated<List<GetAllBooksPaginatedResponse>>> GetAllBooksPaginatedAsync(GetAllBooksPaginatedRequest request);

    }
}
