using AseerAlkotb.Application.Features.Reviews.Requests;
using AseerAlkotb.Application.Features.Reviews.Responses;
using AseerAlkotb.Application.ResponseHandler;


namespace AseerAlkotb.Application.Contracts
{
    public interface IReviewServices
    {
        Task<ApiResponse<AddReviewResponse>> AddReviewAsync(AddReviewRequest request);
        Task<ApiResponse<UpdateReviewResponse>> UpdateReviewAsync(UpdateReviewRequest request);
        Task<ApiResponse<DeleteReviewResponse>> DeleteReviewAsync(DeleteReviewRequest request);
        Task<ApiResponse<GetReviewByIdResponse>> GetReviewByIdAsync(GetReviewByIdRequest request);
        Task<ApiResponsePaginated<List<GetAllReviewsPaginatedResponse>>> GetAllReviewsAsync(GetAllReviewsPaginatedRequest request);
    }
}
