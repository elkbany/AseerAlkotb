using AseerAlkotb.Application.Features.Rag.Requests;
using AseerAlkotb.Application.Features.Rag.Responses;
using AseerAlkotb.Application.ResponseHandler;

namespace AseerAlkotb.Application.Contracts
{
    public interface IRagService
    {
        Task<ApiResponse<RagAskResponse>> AskAsync(RagAskRequest request);                 // يجاوب من الداتا + intent detection
        Task<ApiResponse<string>> GetBookAvailabilityAsync(string bookTitle);
        Task<ApiResponse<List<BookBriefDto>>> GetAuthorBooksAsync(string authorName);
        Task<ApiResponse<List<BookBriefDto>>> GetCategoryBooksAsync(string categoryName);
        Task<ApiResponse<List<BookBriefDto>>> GetCategoryBooksAsync(string categoryName, int take);
        Task<ApiResponse<List<BookBriefDto>>> GetRecommendationsAsync(string query);
    }

}
