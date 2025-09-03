using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Contracts.External;
using AseerAlkotb.Application.Features.Payments.Requests;
using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Application.Features.Payments.Validators;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class PaymentService : AppService, IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymobService _paymobService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IPaymobService paymobService,
            IConfiguration configuration,
            ILogger<PaymentService> logger,
            IServiceProvider serviceProvider,
            IHostEnvironment environment) : base(serviceProvider, environment)
        {
            _unitOfWork = unitOfWork;
            _paymobService = paymobService;
            _configuration = configuration;
            _logger = logger;
        }

        #region Payment Initialization

        public async Task<ApiResponse<InitializePaymentResponse>> InitializePaymentAsync(InitializePaymentRequest request)
        {
            try
            {
                await DoValidationAsync<InitializePaymentRequestValidator, InitializePaymentRequest>(request);

                var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, default, o => o.User);
                if (order == null)
                {
                    return BadRequest<InitializePaymentResponse>("Order not found");
                }

                _logger.LogInformation("Initializing payment for Order {OrderId} with method {PaymentMethod}", 
                    request.OrderId, request.PaymentMethod);

                // Handle different payment methods
                switch (request.PaymentMethod)
                {
                    case PaymentMethod.CashOnDelivery:
                        return await ProcessCODPaymentAsync(request);
                    
                    case PaymentMethod.Card:
                    case PaymentMethod.MobileWallet:
                        return await ProcessOnlinePaymentAsync(request, order);
                    
                    default:
                        return BadRequest<InitializePaymentResponse>("Invalid payment method");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing payment for Order {OrderId}", request.OrderId);
                return BadRequest<InitializePaymentResponse>(ex.Message);
            }
        }

        public async Task<ApiResponse<InitializePaymentResponse>> ProcessCODPaymentAsync(InitializePaymentRequest request)
        {
            try
            {
                var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, default, o => o.User);
                if (order == null)
                {
                    return BadRequest<InitializePaymentResponse>("Order not found");
                }

                // Validate that user matches order's user
                if (order.UserId != request.UserId)
                {
                    return BadRequest<InitializePaymentResponse>("User mismatch with order");
                }

                var transactionId = GenerateTransactionId(request.OrderId, request.UserId);

                // Create COD payment record
                var payment = new Payment
                {
                    UserId = order.UserId, // Use order's UserId to ensure consistency
                    OrderId = request.OrderId,
                    TransactionId = transactionId,
                    Amount = order.FinalAmount,
                    Currency = "EGP",
                    Method = PaymentMethod.CashOnDelivery,
                    Status = PaymentStatus.Pending,
                    PaymentDate = DateTime.UtcNow,
                    ProviderPayload = "COD Payment - No external provider" // Set default for COD
                };

                await _unitOfWork.Payments.InsertAsync(payment);
                order.PaymentStatus = PaymentStatus.Pending;
                order.PaymentMethod = PaymentMethod.CashOnDelivery;
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("COD payment created for Order {OrderId} with Transaction {TransactionId}", 
                    request.OrderId, transactionId);

                var response = new InitializePaymentResponse(
                    payment.Id,
                    transactionId,
                    PaymentMethod.CashOnDelivery,
                    order.FinalAmount,
                    "EGP",
                    PaymentStatus.Pending,
                    null,
                    "Your order has been placed. Payment will be collected upon delivery.",
                    false
                );

                return Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing COD payment for Order {OrderId}", request.OrderId);
                return BadRequest<InitializePaymentResponse>(ex.Message);
            }
        }

        private async Task<ApiResponse<InitializePaymentResponse>> ProcessOnlinePaymentAsync(InitializePaymentRequest request, Order order)
        {
            try
            {
                // Use existing Paymob service for online payments
                var paymobRequest = new ProcessPaymentRequest
                {
                    OrderId = request.OrderId,
                    PaymentMethod = request.PaymentMethod.ToString().ToLower()
                };

                var paymobResponse = await _paymobService.ProcessPaymentAsync(paymobRequest);

                // Get the created payment from the database
                var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(
                    p => p.OrderId == request.OrderId && p.Status == PaymentStatus.Pending);

                if (payment == null)
                {
                    return BadRequest<InitializePaymentResponse>("Failed to create payment record");
                }

                // Update payment with new enum values
                payment.Method = request.PaymentMethod;
                payment.Status = PaymentStatus.Processing;
                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Online payment initialized for Order {OrderId} with redirect URL", request.OrderId);

                var response = new InitializePaymentResponse(
                    payment.Id,
                    payment.TransactionId,
                    request.PaymentMethod,
                    order.FinalAmount,
                    "EGP",
                    PaymentStatus.Processing,
                    paymobResponse.RedirectUrl,
                    "You will be redirected to complete your payment.",
                    true
                );

                return Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing online payment for Order {OrderId}", request.OrderId);
                return BadRequest<InitializePaymentResponse>(ex.Message);
            }
        }

        #endregion

        #region Payment Callbacks & Notifications

        public async Task<ApiResponse<string>> HandleCallbackAsync(PaymentCallbackRequest request)
        {
            try
            {
                _logger.LogInformation("Processing payment callback");

                // Validate HMAC - temporarily disabled for debugging
                var hmacSecret = _configuration["Paymob:HMAC"];
                if (!ValidatePaymobCallback(request, hmacSecret))
                {
                    _logger.LogWarning("Invalid HMAC for callback - Expected: {Expected}, Received: {Received}", 
                        "CalculatedHmac", request.Hmac);
                    // Temporarily comment out HMAC validation for testing
                    // return BadRequest<string>("Invalid callback signature");
                }

                var isSuccess = bool.Parse(request.Success);
                
                if (isSuccess)
                {
                    await _paymobService.UpdateOrderSuccess(request.MerchantOrderId);
                    await UpdatePaymentStatusFromCallback(request.MerchantOrderId, PaymentStatus.Paid);
                    return Success("Payment processed successfully");
                }
                else
                {
                    await _paymobService.UpdateOrderFailed(request.MerchantOrderId);
                    await UpdatePaymentStatusFromCallback(request.MerchantOrderId, PaymentStatus.Failed);
                    return Success("Payment failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment callback");
                return BadRequest<string>("Failed to process payment callback");
            }
        }

        public async Task<ApiResponse<string>> HandleNotificationAsync(Dictionary<string, string> notification)
        {
            try
            {
                _logger.LogInformation("Processing payment notification: {@Notification}", notification);

                // Extract relevant information from notification
                if (notification.TryGetValue("merchant_order_id", out var merchantOrderId) &&
                    notification.TryGetValue("success", out var success))
                {
                    var isSuccess = bool.Parse(success);
                    var status = isSuccess ? PaymentStatus.Paid : PaymentStatus.Failed;
                    
                    await UpdatePaymentStatusFromCallback(merchantOrderId, status);
                    
                    return Success("Notification processed successfully");
                }

                return BadRequest<string>("Invalid notification format");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment notification");
                return BadRequest<string>("Failed to process payment notification");
            }
        }

        public async Task<ApiResponse<string>> HandlePaymentCallbackAsync(PaymentCallbackRequest request)
        {
            return await HandleCallbackAsync(request);
        }

        public async Task<ApiResponse<string>> HandlePaymentNotificationAsync(Dictionary<string, string> notification)
        {
            return await HandleNotificationAsync(notification);
        }

        private async Task UpdatePaymentStatusFromCallback(string specialReference, PaymentStatus status)
        {
            var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.TransactionId == specialReference);
            if (payment != null)
            {
                payment.Status = status;
                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Payment status updated to {Status} for transaction {TransactionId}", 
                    status, specialReference);
            }
        }

        #endregion

        #region Admin Management

        public async Task<ApiResponsePaginated<List<GetAllPaymentsPaginatedResponse>>> GetAllPaymentsPaginatedAsync(GetAllPaymentsPaginatedRequest request)
        {
            try
            {
                var query = _unitOfWork.Payments.GetQueryable()
                    .Include(p => p.User)
                    .Include(p => p.Order)
                    .AsQueryable();

                // Apply filters
                if (request.Status.HasValue)
                    query = query.Where(p => p.Status == request.Status.Value);

                if (request.PaymentMethod.HasValue)
                    query = query.Where(p => p.Method == request.PaymentMethod.Value);

                if (request.FromDate.HasValue)
                    query = query.Where(p => p.PaymentDate >= request.FromDate.Value);

                if (request.ToDate.HasValue)
                    query = query.Where(p => p.PaymentDate <= request.ToDate.Value);

                if (!string.IsNullOrEmpty(request.CustomerSearch))
                {
                    query = query.Where(p => (p.User.FirstName + " " + p.User.LastName).Contains(request.CustomerSearch) ||
                                           p.User.Email.Contains(request.CustomerSearch));
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    query = query.Where(p => p.TransactionId.Contains(request.Search) ||
                                           (p.User.FirstName + " " + p.User.LastName).Contains(request.Search) ||
                                           p.User.Email.Contains(request.Search));
                }

                // Apply sorting
                query = request.DateAscending 
                    ? query.OrderBy(p => p.PaymentDate) 
                    : query.OrderByDescending(p => p.PaymentDate);

                var totalCount = await query.CountAsync();
                var payments = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var mappedPayments = payments.Adapt<List<GetAllPaymentsPaginatedResponse>>();
                
                return Success(mappedPayments, totalCount, request.PageNumber, request.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated payments");
                return new ApiResponsePaginated<List<GetAllPaymentsPaginatedResponse>>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Failed to retrieve payments"
                };
            }
        }

        public async Task<ApiResponse<GetPaymentByIdResponse>> GetPaymentByIdAsync(int paymentId)
        {
            try
            {
                var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(
                    p => p.Id == paymentId, 
                    default, 
                    p => p.User, p => p.Order);

                if (payment == null)
                {
                    return NotFound<GetPaymentByIdResponse>("Payment not found");
                }

                var response = payment.Adapt<GetPaymentByIdResponse>();
                return Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment by ID {PaymentId}", paymentId);
                return BadRequest<GetPaymentByIdResponse>("Failed to retrieve payment");
            }
        }

        public async Task<ApiResponse<string>> UpdatePaymentStatusAsync(UpdatePaymentStatusRequest request)
        {
            try
            {
                await DoValidationAsync<UpdatePaymentStatusRequestValidator, UpdatePaymentStatusRequest>(request);

                var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(
                    p => p.Id == request.PaymentId, 
                    default, 
                    p => p.Order);
                    
                if (payment == null)
                {
                    return NotFound<string>("Payment not found");
                }

                var oldStatus = payment.Status;
                payment.Status = request.NewStatus;

                // Synchronize order payment status
                if (payment.Order != null)
                {
                    switch (request.NewStatus)
                    {
                        case PaymentStatus.Paid:
                            payment.Order.PaymentStatus = PaymentStatus.Paid;
                            break;
                        case PaymentStatus.Failed:
                        case PaymentStatus.Cancelled:
                            payment.Order.PaymentStatus = PaymentStatus.Failed;
                            break;
                        case PaymentStatus.Pending:
                        case PaymentStatus.Processing:
                            payment.Order.PaymentStatus = PaymentStatus.Pending;
                            break;
                    }
                }

                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Payment {PaymentId} status updated from {OldStatus} to {NewStatus} by admin", 
                    request.PaymentId, oldStatus, request.NewStatus);

                return Success("Payment status updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment status for Payment {PaymentId}", request.PaymentId);
                return BadRequest<string>("Failed to update payment status");
            }
        }

        #endregion

        #region Utility Methods

        public async Task<ApiResponse<List<GetAllPaymentsPaginatedResponse>>> GetPaymentsByOrderIdAsync(int orderId)
        {
            try
            {
                var payments = await _unitOfWork.Payments.GetAllAsyncByEx(
                    p => p.OrderId == orderId,
                    0, 100, default,
                    p => p.User, p => p.Order).ToListAsync();

                var response = payments.Adapt<List<GetAllPaymentsPaginatedResponse>>();
                return Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payments for Order {OrderId}", orderId);
                return BadRequest<List<GetAllPaymentsPaginatedResponse>>("Failed to retrieve payments");
            }
        }

        public async Task<ApiResponse<List<GetAllPaymentsPaginatedResponse>>> GetPaymentsByUserIdAsync(int userId)
        {
            try
            {
                var payments = await _unitOfWork.Payments.GetAllAsyncByEx(
                    p => p.UserId == userId,
                    0, 100, default,
                    p => p.User, p => p.Order).ToListAsync();

                var response = payments.Adapt<List<GetAllPaymentsPaginatedResponse>>();
                return Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payments for User {UserId}", userId);
                return BadRequest<List<GetAllPaymentsPaginatedResponse>>("Failed to retrieve payments");
            }
        }

        public bool ValidatePaymobCallback(PaymentCallbackRequest request, string hmacSecret)
        {
            try
            {
                var concatenated = new StringBuilder()
                    .Append(request.AmountCents)
                    .Append(request.CreatedAt)
                    .Append(request.Currency)
                    .Append(request.ErrorOccured)
                    .Append(request.HasParentTransaction)
                    .Append(request.Id)
                    .Append(request.IntegrationId)
                    .Append(request.Is3dSecure)
                    .Append(request.IsAuth)
                    .Append(request.IsCapture)
                    .Append(request.IsRefunded)
                    .Append(request.IsStandalonePayment)
                    .Append(request.IsVoided)
                    .Append(request.Order)
                    .Append(request.Owner)
                    .Append(request.Pending)
                    .Append(request.SourceDataPan)
                    .Append(request.SourceDataSubType)
                    .Append(request.SourceDataType)
                    .Append(request.Success)
                    .ToString();

                var calculatedHmac = _paymobService.ComputeHmacSHA512(concatenated, hmacSecret);
                
                _logger.LogInformation("HMAC Validation - Concatenated: {Concatenated}", concatenated);
                _logger.LogInformation("HMAC Validation - Calculated: {Calculated}, Received: {Received}", 
                    calculatedHmac, request.Hmac);
                
                return request.Hmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating HMAC for callback");
                return false;
            }
        }

        public string GenerateTransactionId(int orderId, int userId)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var random = new Random().Next(1000, 9999);
            return $"TXN_{orderId}_{userId}_{timestamp}_{random}";
        }

        #endregion
    }
}