using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Contracts.External;
using AseerAlkotb.Application.Features.Payments.Mapping;
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
using System.Text.Json;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class PaymentService : AppService, IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymobService _paymobService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;
        private readonly IOrderPaymentSyncService _syncService;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IPaymobService paymobService,
            IConfiguration configuration,
            ILogger<PaymentService> logger,
            IOrderPaymentSyncService syncService,
            IServiceProvider serviceProvider,
            IHostEnvironment environment) : base(serviceProvider, environment)
        {
            _unitOfWork = unitOfWork;
            _paymobService = paymobService;
            _configuration = configuration;
            _logger = logger;
            _syncService = syncService;
        }

        #region Payment Initialization

        public async Task<ApiResponse<InitializePaymentResponse>> InitializePaymentAsync(InitializePaymentRequest request)
        {
            try
            {
                await DoValidationAsync<InitializePaymentRequestValidator, InitializePaymentRequest>(request);

                var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == request.order.Id, default, o => o.User);
                if (order == null)
                {
                    return BadRequest<InitializePaymentResponse>("Order not found");
                }

                _logger.LogInformation("Initializing payment for Order {OrderId} with method {PaymentMethod}", 
                    request.order.Id, order.PaymentMethod);

                // Handle different payment methods based on order's payment method
                switch (order.PaymentMethod)
                {
                    case PaymentMethod.CashOnDelivery:
                        return await ProcessCODPaymentAsync(request);
                    
                    case PaymentMethod.Card:
                    case PaymentMethod.Wallet:
                        return await ProcessOnlinePaymentAsync(request, order);
                    
                    default:
                        return BadRequest<InitializePaymentResponse>("Invalid payment method in order");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing payment for Order {OrderId}", request.order.Id);
                return BadRequest<InitializePaymentResponse>(ex.Message);
            }
        }

        public async Task<ApiResponse<InitializePaymentResponse>> ProcessCODPaymentAsync(InitializePaymentRequest request)
        {
            try
            {
                var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == request.order.Id, default, o => o.User);
                if (order == null)
                {
                    _logger.LogError("Order {OrderId} not found for COD payment initialization", request.order.Id);
                    return BadRequest<InitializePaymentResponse>("Order not found");
                }

                // Validate order state for COD payment
                //if (order.PaymentStatus != PaymentStatus.Pending)
                //{
                //    _logger.LogWarning("COD payment attempted for Order {OrderId} with invalid payment status {Status}", 
                //        request.order.Id, order.PaymentStatus);
                //    return BadRequest<InitializePaymentResponse>("Order payment status is not valid for COD initialization");
                //}

                // Check if payment record already exists
                var existingPayment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.OrderId == order.Id);
                if (existingPayment != null)
                {
                    _logger.LogWarning("Payment record already exists for Order {OrderId}", request.order.Id);
                    return BadRequest<InitializePaymentResponse>("Payment record already exists for this order");
                }

                var transactionId = GenerateTransactionId(request.order.Id, request.order.UserId);

                // Create COD payment record with validation
                var payment = new Payment
                {
                    UserId = order.UserId, // Use order's UserId to ensure consistency
                    OrderId = request.order.Id,
                    TransactionId = transactionId,
                    Amount = order.FinalAmount,
                    Currency = "EGP",
                    Method = order.PaymentMethod, // Take payment method from the order
                    Status = PaymentStatus.Pending,
                    PaymentDate = DateTime.UtcNow,
                    ProviderPayload = "COD Payment - No external provider" // Set default for COD
                };

                await _unitOfWork.Payments.InsertAsync(payment);
                order.PaymentStatus = PaymentStatus.Pending;
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("COD payment created for Order {OrderId} with Transaction {TransactionId}", 
                    request.order.Id, transactionId);

                var response = new InitializePaymentResponse(
                    payment.Id,
                    transactionId,
                    order.PaymentMethod, // Use the payment method from the order
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
                _logger.LogError(ex, "Critical error processing COD payment for Order {OrderId}", request.order.Id);
                return BadRequest<InitializePaymentResponse>("Failed to initialize COD payment due to system error");
            }
        }

        private async Task<ApiResponse<InitializePaymentResponse>> ProcessOnlinePaymentAsync(InitializePaymentRequest request, Order order)
        {
            try
            {
                // Validate order state for online payment
                //if (order.PaymentStatus != PaymentStatus.Pending)
                //{
                //    _logger.LogWarning("Online payment attempted for Order {OrderId} with invalid payment status {Status}", 
                //        request.order.Id, order.PaymentStatus);
                //    return BadRequest<InitializePaymentResponse>("Order payment status is not valid for online payment initialization");
                //}

                // Check if payment record already exists
                var existingPayment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.OrderId == order.Id);
                if (existingPayment != null)
                {
                    _logger.LogWarning("Payment record already exists for Order {OrderId}", request.order.Id);
                    return BadRequest<InitializePaymentResponse>("Payment record already exists for this order");
                }

                // Validate payment method is supported for online payments
                if (order.PaymentMethod != PaymentMethod.Card && order.PaymentMethod != PaymentMethod.Wallet)
                {
                    _logger.LogError("Unsupported payment method {PaymentMethod} for online payment on Order {OrderId}", 
                        order.PaymentMethod, request.order.Id);
                    return BadRequest<InitializePaymentResponse>("Payment method not supported for online payments");
                }

                // Use existing Paymob service for online payments with timeout handling
                var paymobRequest = new ProcessPaymentRequest
                {
                    OrderId = request.order.Id,
                    PaymentMethod = order.PaymentMethod.ToString().ToLower()
                };

                ProcessPaymentResponse paymobResponse;
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)); // 30 second timeout
                    paymobResponse = await _paymobService.ProcessPaymentAsync(paymobRequest);
                    
                    if (paymobResponse == null || string.IsNullOrEmpty(paymobResponse.RedirectUrl))
                    {
                        _logger.LogError("Paymob service returned invalid response for Order {OrderId}", request.order.Id);
                        return BadRequest<InitializePaymentResponse>("Payment gateway returned invalid response");
                    }
                }
                catch (TaskCanceledException)
                {
                    _logger.LogError("Paymob payment initialization timed out for Order {OrderId}", request.order.Id);
                    return BadRequest<InitializePaymentResponse>("Payment gateway timeout. Please try again.");
                }
                catch (HttpRequestException httpEx)
                {
                    _logger.LogError(httpEx, "HTTP error during Paymob payment initialization for Order {OrderId}", request.order.Id);
                    return BadRequest<InitializePaymentResponse>("Payment gateway connection error. Please try again.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error calling Paymob service for Order {OrderId}", request.order.Id);
                    return BadRequest<InitializePaymentResponse>("Payment gateway error. Please try again.");
                }

                // Get the created payment from the database with retry logic
                Payment payment = null;
                var retryCount = 3;
                for (int i = 0; i < retryCount; i++)
                {
                    payment = await _unitOfWork.Payments.FirstOrDefaultAsync(
                        p => p.OrderId == request.order.Id && p.Status == PaymentStatus.Pending);
                    
                    if (payment != null) break;
                    
                    if (i < retryCount - 1)
                    {
                        await Task.Delay(500); // Wait 500ms before retry
                    }
                }

                if (payment == null)
                {
                    _logger.LogError("Failed to retrieve payment record for Order {OrderId} after {RetryCount} attempts", 
                        request.order.Id, retryCount);
                    return BadRequest<InitializePaymentResponse>("Failed to create payment record. Please try again.");
                }

                // Update payment with payment method from order
                payment.Method = order.PaymentMethod;
                payment.Status = PaymentStatus.Processing;
                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Online payment initialized for Order {OrderId} with redirect URL", request.order.Id);

                var response = new InitializePaymentResponse(
                    payment.Id,
                    payment.TransactionId,
                    order.PaymentMethod, // Use payment method from order
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
                _logger.LogError(ex, "Critical error processing online payment for Order {OrderId}", request.order.Id);
                return BadRequest<InitializePaymentResponse>("Failed to initialize online payment due to system error");
            }
        }

        #endregion

        #region Payment Callbacks & Notifications

        public async Task<ApiResponse<string>> HandleCallbackAsync(PaymentCallbackRequest request)
        {
            try
            {
                _logger.LogInformation("Processing payment callback for transaction {TransactionId}", request?.MerchantOrderId);

                // Validate request
                if (request == null)
                {
                    _logger.LogError("Callback request is null");
                    return BadRequest<string>("Invalid callback: Request is null");
                }

                if (string.IsNullOrEmpty(request.MerchantOrderId))
                {
                    _logger.LogError("MerchantOrderId is null or empty in callback");
                    return BadRequest<string>("Invalid callback: MerchantOrderId is missing");
                }

                // Validate HMAC - temporarily disabled for debugging but log validation
                var hmacSecret = _configuration["Paymob:HMAC"];
                var enforceHmac = _configuration.GetValue<bool>("Paymob:EnforceHMAC", false); // Add this config setting
                
                if (!string.IsNullOrEmpty(hmacSecret))
                {
                    if (!ValidatePaymobCallback(request, hmacSecret))
                    {
                        _logger.LogWarning("Invalid HMAC for callback transaction {TransactionId}", request.MerchantOrderId);
                        
                        if (enforceHmac)
                        {
                            return BadRequest<string>("Invalid callback signature");
                        }
                        else
                        {
                            _logger.LogWarning("HMAC enforcement disabled - continuing with callback processing");
                        }
                    }
                    else
                    {
                        _logger.LogInformation("HMAC validation successful for transaction {TransactionId}", request.MerchantOrderId);
                    }
                }
                else
                {
                    _logger.LogWarning("HMAC secret not configured - skipping HMAC validation");
                }

                // Validate payment exists
                var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.TransactionId == request.MerchantOrderId);
                if (payment == null)
                {
                    _logger.LogError("Payment not found for transaction {TransactionId}", request.MerchantOrderId);
                    return BadRequest<string>("Invalid callback: Payment not found");
                }

                // Check if payment is in a valid state for callback processing
                if (payment.Status == PaymentStatus.Paid || payment.Status == PaymentStatus.Failed || payment.Status == PaymentStatus.Cancelled)
                {
                    _logger.LogInformation("Payment {PaymentId} already processed with status {Status}, ignoring callback", 
                        payment.Id, payment.Status);
                    return Success("Payment already processed");
                }

                // Fix: Handle empty or null Success value with safe parsing
                if (string.IsNullOrEmpty(request.Success))
                {
                    _logger.LogError("Success parameter is null or empty in callback for transaction {TransactionId}", request.MerchantOrderId);
                    return BadRequest<string>("Invalid callback: Success parameter is missing");
                }

                if (!bool.TryParse(request.Success, out var isSuccess))
                {
                    _logger.LogError("Invalid Success parameter value: {Success} for transaction {TransactionId}", 
                        request.Success, request.MerchantOrderId);
                    return BadRequest<string>("Invalid callback: Success parameter format is invalid");
                }
                
                // Process callback based on success status
                try
                {
                    if (isSuccess)
                    {
                        await _paymobService.UpdateOrderSuccess(request.MerchantOrderId);
                        await UpdatePaymentStatusFromCallback(request.MerchantOrderId, PaymentStatus.Paid);
                        _logger.LogInformation("Payment callback processed successfully for transaction {TransactionId}", request.MerchantOrderId);
                        return Success("Payment processed successfully");
                    }
                    else
                    {
                        await _paymobService.UpdateOrderFailed(request.MerchantOrderId);
                        await UpdatePaymentStatusFromCallback(request.MerchantOrderId, PaymentStatus.Failed);
                        _logger.LogInformation("Payment failure callback processed for transaction {TransactionId}", request.MerchantOrderId);
                        return Success("Payment failed");
                    }
                }
                catch (KeyNotFoundException knfEx)
                {
                    _logger.LogError(knfEx, "Payment or order not found during callback processing for transaction {TransactionId}", request.MerchantOrderId);
                    return BadRequest<string>("Payment or order not found");
                }
                catch (InvalidOperationException ioEx)
                {
                    _logger.LogError(ioEx, "Invalid operation during callback processing for transaction {TransactionId}", request.MerchantOrderId);
                    return BadRequest<string>("Invalid payment state for callback processing");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error processing payment callback for transaction {TransactionId}", request?.MerchantOrderId);
                return BadRequest<string>("Failed to process payment callback due to system error");
            }
        }

        public async Task<ApiResponse<string>> HandleNotificationAsync(Dictionary<string, string> notification)
        {
            try
            {
                _logger.LogInformation("Processing payment notification: {@Notification}", notification);

                // Validate notification
                if (notification == null || !notification.Any())
                {
                    _logger.LogError("Notification is null or empty");
                    return BadRequest<string>("Invalid notification: Notification data is missing");
                }

                // Extract relevant information from notification
                if (notification.TryGetValue("merchant_order_id", out var merchantOrderId) &&
                    notification.TryGetValue("success", out var success))
                {
                    if (string.IsNullOrEmpty(merchantOrderId))
                    {
                        _logger.LogError("MerchantOrderId is null or empty in notification");
                        return BadRequest<string>("Invalid notification: MerchantOrderId is missing");
                    }

                    if (string.IsNullOrEmpty(success))
                    {
                        _logger.LogError("Success parameter is null or empty in notification");
                        return BadRequest<string>("Invalid notification: Success parameter is missing");
                    }

                    if (!bool.TryParse(success, out var isSuccess))
                    {
                        _logger.LogError("Invalid success parameter in notification: {Success} for transaction {TransactionId}", 
                            success, merchantOrderId);
                        return BadRequest<string>("Invalid notification: Success parameter format is invalid");
                    }

                    // Validate payment exists
                    var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.TransactionId == merchantOrderId);
                    if (payment == null)
                    {
                        _logger.LogError("Payment not found for transaction {TransactionId} in notification", merchantOrderId);
                        return BadRequest<string>("Invalid notification: Payment not found");
                    }

                    // Check if payment is in a valid state for notification processing
                    if (payment.Status == PaymentStatus.Paid || payment.Status == PaymentStatus.Failed || payment.Status == PaymentStatus.Cancelled)
                    {
                        _logger.LogInformation("Payment {PaymentId} already processed with status {Status}, ignoring notification", 
                            payment.Id, payment.Status);
                        return Success("Payment already processed");
                    }
                    
                    var status = isSuccess ? PaymentStatus.Paid : PaymentStatus.Failed;
                    
                    try
                    {
                        await UpdatePaymentStatusFromCallback(merchantOrderId, status);
                        
                        _logger.LogInformation("Payment notification processed successfully for transaction {TransactionId} with status {Status}", 
                            merchantOrderId, status);
                        return Success("Notification processed successfully");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating payment status from notification for transaction {TransactionId}", merchantOrderId);
                        return BadRequest<string>("Failed to update payment status from notification");
                    }
                }
                else
                {
                    _logger.LogError("Required notification parameters missing. Available keys: {Keys}", 
                        string.Join(", ", notification.Keys));
                    return BadRequest<string>("Invalid notification format: Required parameters missing");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error processing payment notification");
                return BadRequest<string>("Failed to process payment notification due to system error");
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
            try
            {
                var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.TransactionId == specialReference);
                if (payment != null)
                {
                    var oldStatus = payment.Status;
                    payment.Status = status;
                    _unitOfWork.Payments.Update(payment);
                    await _unitOfWork.CommitAsync();

                    _logger.LogInformation("Payment status updated to {Status} for transaction {TransactionId}", 
                        status, specialReference);

                    // TODO: Use synchronization service to update order status (when available)
                    /*
                    var syncResult = await _syncService.SyncOrderStatusFromPaymentAsync(payment.OrderId, status, oldStatus);
                    if (!syncResult)
                    {
                        _logger.LogWarning("Failed to synchronize order status for callback Payment {PaymentId} status change from {OldStatus} to {NewStatus}",
                            payment.Id, oldStatus, status);
                    }
                    */
                }
                else
                {
                    _logger.LogWarning("Payment not found for transaction {TransactionId} during callback status update", specialReference);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment status from callback for transaction {TransactionId}", specialReference);
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

                var mappedPayments = payments.ToGetAllPaymentsPaginatedResponseList();
                
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

                var response = payment.ToGetPaymentByIdResponse();
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

                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.CommitAsync();

                // TODO: Use synchronization service to update order status (when available)
                /*
                if (payment.Order != null)
                {
                    var syncResult = await _syncService.SyncOrderStatusFromPaymentAsync(payment.OrderId, request.NewStatus, oldStatus);
                    if (!syncResult)
                    {
                        _logger.LogWarning("Failed to synchronize order status for Payment {PaymentId} status change from {OldStatus} to {NewStatus}",
                            request.PaymentId, oldStatus, request.NewStatus);
                    }
                }
                */

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
                    p => p.User, p => p.Order)
                    .OrderByDescending(p => p.PaymentDate)
                    .ToListAsync();

                var response = payments.ToGetAllPaymentsPaginatedResponseList();
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
                    p => p.User, p => p.Order)
                    .OrderByDescending(p => p.PaymentDate)
                    .ToListAsync();

                var response = payments.ToGetAllPaymentsPaginatedResponseList();
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
                // According to Paymob documentation, the HMAC concatenation should be exactly in this order:
                // amount_cents + created_at + currency + error_occured + has_parent_transaction + id + integration_id + is_3d_secure + is_auth + is_capture + is_refunded + is_standalone_payment + is_voided + order + owner + pending + source_data.pan + source_data.sub_type + source_data.type + success
                
                // Important: All boolean values should be lowercase strings ("true" or "false")
                var concatenated = new StringBuilder()
                    .Append(request.AmountCents ?? "")
                    .Append(request.CreatedAt ?? "")
                    .Append(request.Currency ?? "")
                    .Append(request.ErrorOccured?.ToLower() ?? "")
                    .Append(request.HasParentTransaction?.ToLower() ?? "")
                    .Append(request.Id ?? "")
                    .Append(request.IntegrationId ?? "")
                    .Append(request.Is3dSecure?.ToLower() ?? "")
                    .Append(request.IsAuth?.ToLower() ?? "")
                    .Append(request.IsCapture?.ToLower() ?? "")
                    .Append(request.IsRefunded?.ToLower() ?? "")
                    .Append(request.IsStandalonePayment?.ToLower() ?? "")
                    .Append(request.IsVoided?.ToLower() ?? "")
                    .Append(request.Order ?? "")
                    .Append(request.Owner ?? "")
                    .Append(request.Pending?.ToLower() ?? "")
                    .Append(request.SourceDataPan ?? "")
                    .Append(request.SourceDataSubType ?? "")
                    .Append(request.SourceDataType ?? "")
                    .Append(request.Success?.ToLower() ?? "")
                    .ToString();

                var calculatedHmac = _paymobService.ComputeHmacSHA512(concatenated, hmacSecret);
                
                _logger.LogInformation("HMAC Validation - Concatenated: {Concatenated}", concatenated);
                _logger.LogInformation("HMAC Validation - Calculated: {Calculated}, Received: {Received}", 
                    calculatedHmac, request.Hmac);
                
                var isValid = request.Hmac?.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase) == true;
                _logger.LogInformation("HMAC Validation Result: {IsValid}", isValid ? "Valid ✅" : "Invalid ❌");
                
                if (!isValid)
                {
                    _logger.LogWarning("HMAC Validation Details - Expected: {Expected}, Got: {Received}, String Length: {Length}", 
                        calculatedHmac, request.Hmac, concatenated.Length);
                }
                
                return isValid;
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

        #region Webhook Processing

        public async Task<ApiResponse<string>> ProcessWebhookAsync(PaymentWebhookData webhookData)
        {
            try
            {
                _logger.LogInformation("Processing webhook for transaction {TransactionId} - Success: {Success}", 
                    webhookData.Obj.Id, webhookData.Obj.Success);

                // Find payment in database
                var payment = await FindPaymentForWebhook(webhookData);
                
                if (payment == null)
                {
                    _logger.LogError("Payment not found for webhook transaction {TransactionId} with amount {Amount}", 
                        webhookData.Obj.Id, webhookData.Obj.AmountCents / 100.0);
                    return BadRequest<string>("Payment not found for this transaction");
                }

                _logger.LogInformation("Found payment {PaymentId} for transaction {TransactionId}", 
                    payment.Id, webhookData.Obj.Id);

                // Check if payment is already processed
                if (payment.Status == PaymentStatus.Paid || payment.Status == PaymentStatus.Failed || payment.Status == PaymentStatus.Cancelled)
                {
                    _logger.LogInformation("Payment {PaymentId} already processed with status {Status}", 
                        payment.Id, payment.Status);
                    return Success("Payment already processed");
                }

                // Update payment status
                var newStatus = webhookData.Obj.Success ? PaymentStatus.Paid : PaymentStatus.Failed;
                var oldStatus = payment.Status;

                payment.Status = newStatus;
                payment.PaymobOrderId = webhookData.Obj.Order.Id;
                
                // Save webhook data in ProviderPayload
                payment.ProviderPayload = JsonSerializer.Serialize(webhookData, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower 
                });

                _unitOfWork.Payments.Update(payment);

                // Update order payment status as well
                var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == payment.OrderId);
                if (order != null)
                {
                    order.PaymentStatus = newStatus;
                    _unitOfWork.Orders.Update(order);
                    _logger.LogInformation("Updated order {OrderId} payment status to {Status}", 
                        order.Id, newStatus);
                }

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Payment {PaymentId} status updated from {OldStatus} to {NewStatus}", 
                    payment.Id, oldStatus, newStatus);

                // Sync order status with payment status
                var syncResult = await _syncService.SyncOrderStatusFromPaymentAsync(payment.OrderId, newStatus, oldStatus);
                if (!syncResult.HasValue)
                {
                    _logger.LogWarning("Failed to sync order status for payment {PaymentId}", payment.Id);
                }

                return Success($"Payment updated successfully to {newStatus}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook for transaction {TransactionId}", 
                    webhookData?.Obj?.Id);
                return BadRequest<string>("Processing failed due to system error");
            }
        }

        private async Task<Payment?> FindPaymentForWebhook(PaymentWebhookData webhookData)
        {
            // Method 1: Find by PaymobOrderId if stored
            var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(
                p => p.PaymobOrderId == webhookData.Obj.Order.Id);
            
            if (payment != null)
            {
                _logger.LogInformation("Found payment by PaymobOrderId: {PaymobOrderId}", 
                    webhookData.Obj.Order.Id);
                return payment;
            }

            // Method 2: Find by amount and time
            var amountInEGP = webhookData.Obj.AmountCents / 100m;
            
            DateTime webhookTime;
            try
            {
                webhookTime = DateTime.Parse(webhookData.Obj.CreatedAt);
            }
            catch
            {
                webhookTime = DateTime.UtcNow; // fallback
            }
            
            var timeBuffer = TimeSpan.FromHours(2); // 2 hour buffer

            var candidatePayments = await _unitOfWork.Payments.GetAllAsyncByEx(p => 
                Math.Abs(p.Amount - amountInEGP) < 0.01m && // Same amount (with small tolerance)
                p.PaymentDate >= webhookTime.Subtract(timeBuffer) && 
                p.PaymentDate <= webhookTime.Add(timeBuffer) &&
                (p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Processing), 
                0, 5)
                .ToListAsync();

            payment = candidatePayments.FirstOrDefault();

            if (payment != null)
            {
                _logger.LogInformation("Found payment by amount {Amount} and time matching", amountInEGP);
                
                // Save PaymobOrderId for future reference
                payment.PaymobOrderId = webhookData.Obj.Order.Id;
                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("No matching payment found for amount {Amount} at time {Time}", 
                    amountInEGP, webhookTime);
            }

            return payment;
        }

        #endregion
    }
}