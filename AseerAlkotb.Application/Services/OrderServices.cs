﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Books.DTOs;
using AseerAlkotb.Application.Features.Orders.Filters;
using AseerAlkotb.Application.Features.Orders.Requests;
using AseerAlkotb.Application.Features.Orders.Responses;
using AseerAlkotb.Application.Features.Orders.Validators;
using AseerAlkotb.Application.Features.Payments.Requests;
using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AseerAlkotb.Application.Services
{
    public class OrderServices : AppService, IOrderServices
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<User> userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IPaymentService _paymentService;
        private readonly IOrderPaymentSyncService _syncService;

        public OrderServices(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IHostEnvironment environment, IPaymentService paymentService, IOrderPaymentSyncService syncService) : base(serviceProvider, environment)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _syncService = syncService;
        }

        #region Checkout (Place Order)
        public async Task<ApiResponse<AddOrderResponse>> CheckoutAsync(AddOrderRequest request)
        {
            await DoValidationAsync<AddOrderRequestValidator, AddOrderRequest>(request);

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<AddOrderResponse>();
            }

            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<AddOrderResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<AddOrderResponse>();
            }

            // Validate cart exists and has items - use current user's ID for security
            var cart = await GetCartWithItemsAsync(currentUser.Id);
            if (!ValidateCartNotEmpty(cart))
                return NotFound<AddOrderResponse>($"{_stringLocalizer["Cart"]} {_stringLocalizer["NotFound"]}");

            // Get current book prices from database
            var books = await GetBooksForCartAsync(cart);

            // Use database transaction for atomicity
            using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                // Create and populate order with current user's ID
                var order = await CreateOrderAsync(request, cart, books, currentUser);

                // Save order first to get ID
                await unitOfWork.Orders.InsertAsync(order);
                await unitOfWork.SaveChangesAsync(); // Save to get order ID

                // Initialize payment - this will validate payment method and create payment record
                var paymentInitResult = await _paymentService.InitializePaymentAsync(new InitializePaymentRequest(order));
                if (!paymentInitResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return BadRequest<AddOrderResponse>($"Payment initialization failed: {paymentInitResult.Message}");
                }

                // Clear cart and commit transaction
                cart.CartItems.Clear();
                unitOfWork.Carts.Update(cart);
                await unitOfWork.SaveChangesAsync();
                
                await transaction.CommitAsync();

                // Return successful response with payment info
                var response = new AddOrderResponse(
                    order.Id,
                    order.TrackingNumber,
                    new PaymentInitializationInfo(
                        paymentInitResult.Data.PaymentId,
                        paymentInitResult.Data.TransactionId,
                        paymentInitResult.Data.PaymentMethod,
                        paymentInitResult.Data.Amount,
                        paymentInitResult.Data.Currency,
                        paymentInitResult.Data.Status,
                        paymentInitResult.Data.RedirectUrl,
                        paymentInitResult.Data.Instructions,
                        paymentInitResult.Data.RequiresRedirect
                    )
                );
                
                return Success(response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest<AddOrderResponse>($"Checkout failed: {ex.Message}");
            }
        }

        private async Task<Cart> GetCartWithItemsAsync(int userId)
        {
            return await unitOfWork.Carts.FirstOrDefaultAsync(
                c => c.UserId == userId,
                default,
                c => c.CartItems
            );
        }

        private static bool ValidateCartNotEmpty(Cart cart)
        {
            if (cart == null || !cart.CartItems.Any())
                return false;
            return true;
        }

        private async Task<List<Book>> GetBooksForCartAsync(Cart cart)
        {
            var bookIds = cart.CartItems.Select(ci => ci.BookId).ToList();
            return await unitOfWork.Books.GetByIdsAsync(bookIds);
        }

        private async Task<Order> CreateOrderAsync(AddOrderRequest request, Cart cart, List<Book> books, User user)
        {
            // Map request to order entity first
            var order = request.Adapt<Order>();
            
            // Set user relationship
            order.User = user;
            
            // IMPORTANT: Explicitly set status AFTER mapping to override any defaults
            order.Status = OrderStatus.Pending;  // This MUST be 1, not 0
            order.PaymentStatus = PaymentStatus.Pending;
            order.OrderDate = DateTime.UtcNow;
            
            // Add order items with current prices
            AddOrderItems(order, cart.CartItems, books);

            // Calculate costs
            await CalculateOrderCostsAsync(order, request);

            // Generate tracking number
            order.TrackingNumber = await GenerateUniqueTrackingNumberAsync();

            return order;
        }

        private static void AddOrderItems(Order order, IEnumerable<CartItem> cartItems, List<Book> books)
        {
            foreach (var cartItem in cartItems)
            {
                var book = books.First(b => b.Id == cartItem.BookId);

                order.OrderItems.Add(new OrderItem
                {
                    BookId = book.Id,
                    Book=book,
                    Order=order,
                    OrderId=order.Id,
                    UnitPrice = book.Price, // Always use current price from DB
                    Quantity = cartItem.Quantity
                   
                });
            }
        }

        private async Task CalculateOrderCostsAsync(Order order, AddOrderRequest request)
        {
            // Calculate base total
            order.TotalAmount = order.OrderItems.Sum(oi => oi.UnitPrice);

            // Calculate shipping
            order.ShippingCost = await ShippingServices.CalculateShippingCostAsync(request, order.TotalAmount, unitOfWork);

            // Final Amount and Calculate discount
            order.FinalAmount =
               (order.ShippingCost + order.TotalAmount) - CalculateDiscountAmount(order);
        }

        private static decimal CalculateDiscountAmount(Order order)
        {
            var discountedTotal = order.OrderItems.Sum(oi => oi.Book.DiscountedPrice);
            order.DiscountAmount = order.TotalAmount - discountedTotal;

            // Handle edge case where discount equals total (likely means no discount)
            if (order.DiscountAmount == order.TotalAmount)
            {
                order.DiscountAmount = 0;
            }
            return order.DiscountAmount;
        }

        private async Task<string> GenerateUniqueTrackingNumberAsync()
        {
            string trackingNumber;
            bool exists;

            do
            {
                trackingNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{GenerateRandomString(6)}";
                exists = await unitOfWork.Orders.AnyAsync(o => o.TrackingNumber == trackingNumber);
            }
            while (exists);

            return trackingNumber;
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion

        public async Task<ApiResponse<CancelOrderResponse>> CancelOrderAsync(CancelOrderRequest request)
        {
            await DoValidationAsync<CancelOrderRequestValidator, CancelOrderRequest>(request);

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<CancelOrderResponse>();
            }

            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<CancelOrderResponse>();
            }

            // Check user roles
            var isAdmin = await userManager.IsInRoleAsync(currentUser, "Admin");
            var isClient = await userManager.IsInRoleAsync(currentUser, "Client");

            if (!isAdmin && !isClient)
            {
                return UnAuthorized<CancelOrderResponse>();
            }

            var order = await unitOfWork.Orders.FirstOrDefaultAsync(o => o.TrackingNumber == request.TrackingNumber);
            if (order == null)
            {
                return NotFound<CancelOrderResponse>($"{_stringLocalizer["Order"]} {_stringLocalizer["NotFound"]}");
            }

            // Client-specific authorization and business rules
            if (isClient && !isAdmin)
            {
                // Client can only cancel their own orders
                if (order.UserId != currentUser.Id)
                {
                    return UnAuthorized<CancelOrderResponse>();
                }

                // Client can only cancel if status is Pending or Approved
                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Approved)
                {
                    return BadRequest<CancelOrderResponse>($"{_stringLocalizer["CannotCancelOrder"]} - {_stringLocalizer["InvalidStatusForCancellation"]}");
                }
            }

            // Admin-specific business rules
            if (isAdmin)
            {
                // Admin cannot cancel delivered orders
                if (order.Status == OrderStatus.Delivered)
                {
                    return BadRequest<CancelOrderResponse>($"{_stringLocalizer["CannotCancelOrder"]} - {_stringLocalizer["OrderAlreadyDelivered"]}");
                }
                // Admin can cancel Shipped orders (if possible in business logic)
                // No additional restrictions for admin on other statuses
            }

            // Perform cancellation
            order.Status = OrderStatus.Cancelled;

            unitOfWork.Orders.Update(order);
            await unitOfWork.CommitAsync();

            var ordMap = order.Adapt<CancelOrderResponse>();
            return Success(ordMap);
        }

        public async Task<ApiResponsePaginated<List<GetAllOrdersPaginatedResponse>>> GetAllOrdersPaginatedByAdminAsync(GetAllOrdersPaginatedRequest request)
        {
            await DoValidationAsync<GetAllOrdersPaginatedRequestValidator, GetAllOrdersPaginatedRequest>(request);
            var orders = await unitOfWork.Orders.GetAllAsync((request.PageNumber - 1) * request.PageSize, request.PageSize, default, o => o.OrderItems, o => o.User, o => o.Governorate, o => o.City)
                .Filter(request)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            var totalCount = await unitOfWork.Orders.CountAsync();

            // Manual mapping instead of Adapt<>
            var ordsMap = orders.Select(order => new GetAllOrdersPaginatedResponse(
                order.Id,
                order.User?.UserName ?? string.Empty,
                order.PaymentMethod,
                order.PaymentStatus,
                order.GovernorateId,
                order.Governorate?.Name ?? string.Empty,
                order.CityId,
                order.City?.Name ?? string.Empty,
                order.Status,
                order.TrackingNumber,
                order.FinalAmount,
                order.OrderDate,
                order.OrderItems
                    .Where(oi => oi.Book != null)
                    .Select(oi => new BookDTO(
                        oi.Book.Title,
                        oi.UnitPrice,
                        oi.Quantity
                    ))
                    .ToList()
            )).ToList();

            return Success(ordsMap, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<ApiResponsePaginated<List<GetAllUserOrdersPaginatedResponse>>> GetAllUserOrdersPaginatedAsync(GetAllUserOrdersPaginatedRequest request)
        {
            await DoValidationAsync<GetAllUserOrdersPaginatedRequestValidator, GetAllUserOrdersPaginatedRequest>(request);
            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorizedList<GetAllUserOrdersPaginatedResponse>();
            }
            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorizedList<GetAllUserOrdersPaginatedResponse>();
            }
            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorizedList<GetAllUserOrdersPaginatedResponse>();
            }
            // Use current user's ID for security instead of request.UserId
            var orders =  unitOfWork.Orders.GetAllAsyncByEx(o => o.UserId == currentUser.Id, (request.PageNumber - 1) * request.PageSize, request.PageSize, default, o => o.OrderItems, o => o.Governorate, o => o.City);
            var totalCount = await unitOfWork.Orders.CountAsync(o => o.UserId == currentUser.Id);

            // Manual mapping instead of Adapt<>
            var ordsMap = orders.Select(order => new GetAllUserOrdersPaginatedResponse(
                order.Id,
                order.User.UserName ?? string.Empty,
                order.PaymentMethod,
                order.PaymentStatus,
                order.GovernorateId,
                order.Governorate.Name ?? string.Empty,
                order.CityId,
                order.City.Name ?? string.Empty,
                order.Status,
                order.TrackingNumber,
                order.FinalAmount,
                order.OrderDate,
                order.OrderItems
                    .Where(oi => oi.Book != null)
                    .Select(oi => new BookDTO(
                        oi.Book.Title,
                        oi.UnitPrice,
                        oi.Quantity
                    ))
                    .ToList()
            )).ToList();

            return Success(ordsMap, totalCount, request.PageNumber, request.PageSize);
        }

        // get by tracking number
        public async Task<ApiResponse<GetOrderByAdminByTrackingNumberResponse>> GetOrderByTrackingNumberByAdminAsync(GetOrderByAdminByTrackingNumberRequest request)
        {
            await DoValidationAsync<GetOrderByAdminByTrackingNumberRequestValidator, GetOrderByAdminByTrackingNumberRequest>(request);
            var query = unitOfWork.Orders.GetQueryable(
                o => o.TrackingNumber == request.TrackingNumber,
                q => q.Include(o => o.User)
                      .Include(o => o.OrderItems)
                      .ThenInclude(oi => oi.Book)
                      .Include(o => o.Governorate)
                      .Include(o => o.City)
            );
            var order = await query.FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound<GetOrderByAdminByTrackingNumberResponse>($"{_stringLocalizer["Order"]} {_stringLocalizer["NotFound"]}");
            }
            var ordMap = new GetOrderByAdminByTrackingNumberResponse(
     order.Id,
     order.User?.UserName ?? string.Empty, // keep nullable-safe
     order.PaymentMethod,
     order.PaymentStatus,
     order.GovernorateId,
     order.Governorate?.Name ?? string.Empty,
     order.CityId,
     order.City?.Name ?? string.Empty,
     order.Status,
     order.TrackingNumber,
     order.TotalAmount,
     order.ShippingCost,
     order.DiscountAmount,
     order.FinalAmount,
     order.OrderDate,
     order.UpdatedAt,
     order.OrderItems
         .Where(oi => oi.Book != null)
         .Select(oi => new BookDTO(
             oi.Book.Title,
            oi.UnitPrice,
            oi.Quantity
         ))
         .ToList()
 );
            return Success(ordMap);
        }

        public async Task<ApiResponse<GetUserOrderByTrackingNumberResponse>> GetOrderByTrackingNumberByUserAsync(GetUserOrderByTrackingNumberRequest request)
        {
            await DoValidationAsync<GetUserOrderByTrackingNumberRequestValidator, GetUserOrderByTrackingNumberRequest>(request);

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<GetUserOrderByTrackingNumberResponse>();
            }

            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<GetUserOrderByTrackingNumberResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<GetUserOrderByTrackingNumberResponse>();
            }

            var order = await unitOfWork.Orders.FirstOrDefaultAsync(o => o.TrackingNumber == request.TrackingNumber, default, o => o.OrderItems, o => o.Governorate, o => o.City);
            if (order == null)
            {
                return NotFound<GetUserOrderByTrackingNumberResponse>($"{_stringLocalizer["Order"]} {_stringLocalizer["NotFound"]}");
            }

            // Ensure user can only access their own orders
            if (order.UserId != currentUser.Id)
            {
                return UnAuthorized<GetUserOrderByTrackingNumberResponse>();
            }

            var ordMap = new GetUserOrderByTrackingNumberResponse(
                order.Id,
                order.User?.UserName ?? string.Empty,
                order.PaymentMethod,
                order.PaymentStatus,
                order.GovernorateId,
                order.Governorate?.Name ?? string.Empty,
                order.CityId,
                order.City?.Name ?? string.Empty,
                order.Status,
                order.TrackingNumber,
                order.FinalAmount,
                order.OrderDate,
                order.OrderItems
                    .Where(oi => oi.Book != null)
                    .Select(oi => new BookDTO(
                        oi.Book.Title,
                        oi.UnitPrice,
                        oi.Quantity
                    ))
                    .ToList()
            );

            return Success(ordMap);
        }

        public async Task<ApiResponse<UpdateOrderStatusResponse>> UpdateOrderStatusAsync(UpdateOrderStatusRequest request)
        {
            await DoValidationAsync<UpdateOrderStatusRequestValidator, UpdateOrderStatusRequest>(request);

            var order = await unitOfWork.Orders.FirstOrDefaultAsync(
                o => o.Id == request.OrderId,
                default,
                o => o.User, o => o.OrderItems);

            if (order == null)
            {
                return NotFound<UpdateOrderStatusResponse>($"{_stringLocalizer["Order"]} {_stringLocalizer["NotFound"]}");
            }

            // Check if order is already delivered - cannot change status from delivered
            if (order.Status == OrderStatus.Delivered)
            {
                return BadRequest<UpdateOrderStatusResponse>($"{_stringLocalizer["CannotChangeDeliveredOrder"]}");
            }

            // NEW: Check payment status for status transitions that require payment
            if (!await CanUpdateToStatusAsync(order, request.NewStatus))
            {
                return BadRequest<UpdateOrderStatusResponse>($"{_stringLocalizer["PaymentRequiredForStatusChange"]}");
            }

            // Validate status transition (your existing logic)
            if (!IsValidStatusTransition(order.Status, request.NewStatus))
            {
                return BadRequest<UpdateOrderStatusResponse>($"{_stringLocalizer["CannotChangeStatus"]} {order.Status} {_stringLocalizer["To"]} {request.NewStatus}");
            }

            var oldStatus = order.Status;

            // NEW: Handle stock restoration if cancelling order
            if (request.NewStatus == OrderStatus.Cancelled && oldStatus != OrderStatus.Cancelled)
            {
                await RestoreStockForCancelledOrderAsync(order);
            }

            order.Status = request.NewStatus;

            // Update order first
            unitOfWork.Orders.Update(order);
            await unitOfWork.CommitAsync();

            // Use synchronization service to update payment status
            var syncResult = await _syncService.SyncPaymentStatusFromOrderAsync(request.OrderId, request.NewStatus, oldStatus);
            if (!syncResult)
            {
                // Log warning but don't fail the order update
                // The synchronization service already logs the specific error
                // We could implement compensation logic here if needed
            }

            // For now, use the old logic for COD orders
            if (order.PaymentMethod == PaymentMethod.CashOnDelivery)
            {
                var payment = await unitOfWork.Payments.FirstOrDefaultAsync(p => p.OrderId == order.Id);
                if (payment != null)
                {
                    switch (request.NewStatus)
                    {
                        case OrderStatus.Delivered:
                            payment.Status = PaymentStatus.Paid;
                            order.PaymentStatus = PaymentStatus.Paid;
                            break;
                        case OrderStatus.Cancelled:
                            payment.Status = PaymentStatus.Cancelled;
                            order.PaymentStatus = PaymentStatus.Cancelled;
                            break;
                    }
                    unitOfWork.Payments.Update(payment);
                    await unitOfWork.CommitAsync();
                }
            }

            var response = new UpdateOrderStatusResponse(
                order.Id,
                order.Status,
                order.TrackingNumber,
                DateTime.UtcNow
            );

            return Success(response);
        }

        // NEW: Method to check if order can be updated to the requested status based on payment
        private async Task<bool> CanUpdateToStatusAsync(Order order, OrderStatus newStatus)
        {
            // These statuses require payment to be completed first
            var statusesRequiringPayment = new[]
            {
        OrderStatus.Approved,
        OrderStatus.Shipped,
        OrderStatus.Delivered
    };

            if (!statusesRequiringPayment.Contains(newStatus))
            {
                return true; // No payment requirement for other statuses
            }

            // Special handling for Cash on Delivery - allow status changes even if payment is pending
            if (order.PaymentMethod == PaymentMethod.CashOnDelivery)
            {
                return true;
            }

            // For all other payment methods, check if payment is completed
            var payment = await unitOfWork.Payments.FirstOrDefaultAsync(p => p.OrderId == order.Id);

            return payment != null && payment.Status == PaymentStatus.Paid;
        }

        // NEW: Method to restore stock when order is cancelled
        private async Task RestoreStockForCancelledOrderAsync(Order order)
        {
            if (order.OrderItems == null || !order.OrderItems.Any())
            {
                return;
            }

            // Get all book IDs from the order
            var bookIds = order.OrderItems.Select(oi => oi.BookId).ToList();
            var books = await unitOfWork.Books.GetByIdsAsync(bookIds);

            // Restore stock for each book
            foreach (var orderItem in order.OrderItems)
            {
                var book = books.FirstOrDefault(b => b.Id == orderItem.BookId);
                if (book != null)
                {
                    book.StockQuantity += orderItem.Quantity;
                    unitOfWork.Books.Update(book);
                }
            }

            await unitOfWork.CommitAsync();
        }

        private static bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            // If trying to set the same status, it's valid (no change needed but not an error)
            if (currentStatus == newStatus)
                return true;

            // Define valid transitions for each status
            return currentStatus switch
            {
                OrderStatus.Pending => newStatus is
                    OrderStatus.Approved or
                    OrderStatus.Cancelled,
                OrderStatus.Approved => newStatus is
                    OrderStatus.Shipped or
                    OrderStatus.Cancelled,
                OrderStatus.Shipped => newStatus is
                    OrderStatus.Delivered or
                    OrderStatus.Cancelled,
                OrderStatus.Delivered => newStatus is
                    OrderStatus.Delivered,
                OrderStatus.Cancelled => false,
                _ => false // Invalid current status
            };
        }
    }
}