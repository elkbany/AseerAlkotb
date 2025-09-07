using System.Threading.Tasks;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Application.Features.Chat.Requests;
using AseerAlkotb.Application.Features.Chat.Responses;

namespace AseerAlkotb.Application.Contracts
{
    public interface IChatService
    {
        Task<ApiResponse<ChatResponse>> AskAsync(ChatRequest request);
    }
}



