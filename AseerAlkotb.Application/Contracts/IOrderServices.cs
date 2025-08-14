using AseerAlkotb.Application.Features.Orders.Requests;
using AseerAlkotb.Application.Features.Orders.Responses;
using AseerAlkotb.Application.ResponseHandler;


namespace AseerAlkotb.Application.Contracts
{
    public interface IOrderServices
    {
        Task<ApiResponse<AddOrderResponse>> CheckoutAsync(AddOrderRequest request);

        Task<ApiResponse<CancelOrderResponse>> CancelOrderAsync(CancelOrderRequest request);

        Task<ApiResponsePaginated<List<GetAllOrdersPaginatedResponse>>> GetAllOrdersPaginatedByAdminAsync(GetAllOrdersPaginatedRequest request);

        Task<ApiResponsePaginated<List<GetAllUserOrdersPaginatedResponse>>> GetAllUserOrdersPaginatedAsync(GetAllUserOrdersPaginatedRequest request);

        Task<ApiResponse<GetOrderByAdminByTrackingNumberResponse>> GetOrderByTrackingNumberByAdminAsync(GetOrderByAdminByTrackingNumberRequest request);

        Task<ApiResponse<GetUserOrderByTrackingNumberResponse>> GetOrderByTrackingNumberByUserAsync(GetUserOrderByTrackingNumberRequest request);
    }
}
