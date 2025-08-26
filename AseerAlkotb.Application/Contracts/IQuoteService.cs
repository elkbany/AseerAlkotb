using AseerAlkotb.Application.Features.Quotes.Requests;
using AseerAlkotb.Application.Features.Quotes.Responses;
using AseerAlkotb.Application.ResponseHandler;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Contracts
{
    public interface IQuoteService
    {
        Task<ApiResponse<AddQuoteResponse>> AddQuoteAsync(AddQuoteRequest request);

        Task<ApiResponse<UpdateQuoteResponse>> UpdateQuoteAsync(UpdateQuoteRequest request);

        Task<ApiResponse<DeleteQuoteResponse>> DeleteQuoteAsync(DeleteQuoteRequest request);

        Task<ApiResponse<GetQuoteByIdResponse>> GetQuoteByIdAsync(GetQuoteByIdRequest request);

        Task<ApiResponse<List<GetAllQuotesPaginatedResponse>>> GetAllQuotesAsync(GetAllQuotesPaginatedRequest request);
    }
}
