using AseerAlkotb.Application.Features.Payments.Requests;
using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Application.ResponseHandler;

namespace AseerAlkotb.Application.Contracts
{
    public interface IPaymentService
    {
        // Payment Initialization
        Task<ApiResponse<InitializePaymentResponse>> InitializePaymentAsync(InitializePaymentRequest request);
        
        // Payment Callbacks & Notifications
        Task<ApiResponse<string>> HandlePaymentCallbackAsync(PaymentCallbackRequest request);
        Task<ApiResponse<string>> HandlePaymentNotificationAsync(Dictionary<string, string> notification);
        
        // Cash on Delivery
        Task<ApiResponse<InitializePaymentResponse>> ProcessCODPaymentAsync(InitializePaymentRequest request);
        
        // Admin Management
        Task<ApiResponsePaginated<List<GetAllPaymentsPaginatedResponse>>> GetAllPaymentsPaginatedAsync(GetAllPaymentsPaginatedRequest request);
        Task<ApiResponse<GetPaymentByIdResponse>> GetPaymentByIdAsync(int paymentId);
        Task<ApiResponse<string>> UpdatePaymentStatusAsync(UpdatePaymentStatusRequest request);
        
        // Utility Methods
        Task<ApiResponse<List<GetAllPaymentsPaginatedResponse>>> GetPaymentsByOrderIdAsync(int orderId);
        Task<ApiResponse<List<GetAllPaymentsPaginatedResponse>>> GetPaymentsByUserIdAsync(int userId);
        
        // Validation & Security
        bool ValidatePaymobCallback(PaymentCallbackRequest request, string hmacSecret);
        string GenerateTransactionId(int orderId, int userId);
    }
}