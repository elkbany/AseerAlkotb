using AseerAlkotb.Application.Features.Rag.Requests;
using AseerAlkotb.Application.Features.Rag.Responses;
using AseerAlkotb.Application.ResponseHandler;

namespace AseerAlkotb.Application.Contracts
{
    public interface IRagService
    {
            Task<ApiResponse<RagAskResponse>> AskAsync(RagAskRequest request);

    }

}
