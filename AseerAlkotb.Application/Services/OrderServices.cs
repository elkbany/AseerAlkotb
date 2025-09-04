﻿﻿﻿﻿using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Books.DTOs;
using AseerAlkotb.Application.Features.Orders.Filters;
using AseerAlkotb.Application.Features.Orders.Requests;
using AseerAlkotb.Application.Features.Orders.Responses;
using AseerAlkotb.Application.Features.Orders.Validators;
using AseerAlkotb.Application.Features.Payments.Requests;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;



namespace AseerAlkotb.Application.Services
{
    public class OrderServices : AppService,IOrderServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderServices(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IHostEnvironment environment, IPaymentService paymentService) : base(serviceProvider, environment)
        {
            this.unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        #region Checkout Unorganised version
        //public async Task<ApiResponse<AddOrderResponse>> AddOrderAsync(AddOrderRequest request)
        //{
        //    await DoValidationAsync<AddOrderRequestValidator,AddOrderRequest>(request);
        //    //check if the current user is login 

        //    var order = request.Adapt<Order>();

        //}
        //public async Task<ApiResponse<AddOrderResponse>> CheckoutAsync(AddOrderRequest request)
        //{

        //    var cart = await unitOfWork.Carts.FirstOrDefaultAsync(c=>c.UserId==request.UserId,default,c=>c.CartItems);
        //    if (cart == null || !cart.CartItems.Any())
        //        throw new InvalidOperationException("Cart is empty.");

        //    // Load current prices from DB
        //    var bookIds = cart.CartItems.Select(ci => ci.BookId).ToList();
        //    var books = await unitOfWork.Books.GetByIdsAsync(bookIds);


        //    var order = request.Adapt<Order>();

        //    foreach (var cartItem in cart.CartItems)
        //    {
        //        var book = books.FirstOrDefault(b => b.Id == cartItem.BookId);
        //        order.OrderItems.Add(new OrderItem
        //        {
        //            BookId = book.Id,
        //            UnitPrice = book.Price, // Always from DB
        //            Quantity = cartItem.Quantity
        //        });
        //    }
        //    order.ShippingCost = ShippingServices.CalculateShippingCost(request);
        //    order.TotalAmount = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);
        //    order.TaxAmount = order.TotalAmount *  0.14m;
        //    order.DiscountAmount = order.TotalAmount - (order.OrderItems.Sum(oi => oi.Book.DiscountedPrice * oi.Quantity));
        //    if (order.DiscountAmount == order.TotalAmount)
        //    {

        //        order.DiscountAmount = 0;
        //    }
        //    order.TrackingNumber = await GenerateUniqueTrackingNumberAsync();
        //    await unitOfWork.Orders.InsertAsync(order);
        //    cart.CartItems.Clear();
        //     unitOfWork.Carts.Update(cart);

        //    await unitOfWork.CommitAsync();
        //    var ordMap = order.Adapt<AddOrderResponse>();
        //    return Success(ordMap);
        //}
        //private async Task<string> GenerateUniqueTrackingNumberAsync()
        //{
        //    string trackingNumber;
        //    bool exists;

        //    do
        //    {
        //        // Example format: ORD-20250813-AB1234
        //        trackingNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{GenerateRandomString(6)}";

        //        exists = await unitOfWork.Orders
        //            .AnyAsync(o => o.TrackingNumber == trackingNumber);

        //    } while (exists);

        //    return trackingNumber;
        //} 
        #endregion

        #region Checkout (Place Order)
        public async Task<ApiResponse<AddOrderResponse>> CheckoutAsync(AddOrderRequest request)
        {
            await DoValidationAsync<AddOrderRequestValidator, AddOrderRequest>(request);
            // Validate cart exists and has items
            var cart = await GetCartWithItemsAsync(request.UserId);
            if (!ValidateCartNotEmpty(cart))
                return NotFound<AddOrderResponse>("Cart not found");

            // Get current book prices from database
            var books = await GetBooksForCartAsync(cart);

            // Create and populate order
            var order = await CreateOrderAsync(request, cart, books);

            // Process checkout transaction
            await ProcessCheckoutTransactionAsync(order, cart);

            // Return response
            var response = order.Adapt<AddOrderResponse>();
            await _paymentService.InitializePaymentAsync(new InitializePaymentRequest();
            return Success(response);
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

        private async Task<Order> CreateOrderAsync(AddOrderRequest request, Cart cart, List<Book> books)
        {
            var order = request.Adapt<Order>();

            // Add order items with current prices
            AddOrderItems(order, cart.CartItems, books);

            // Calculate costs
            CalculateOrderCosts(order, request);

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
                    UnitPrice = book.Price, // Always use current price from DB
                    Quantity = cartItem.Quantity
                });
            }
        }

        private void CalculateOrderCosts(Order order, AddOrderRequest request)
        {
            // Calculate base total
            order.TotalAmount = order.OrderItems.Sum(oi => oi.TotalPrice);

            // Calculate shipping
            order.ShippingCost = ShippingServices.CalculateShippingCost(request);

            // Calculate tax (14%)
            order.TaxAmount = order.TotalAmount * 0.14m;

            // Final Amount and Calculate discount
            order.FinalAmount=
                CalculateDiscountAmount(order)+order.TaxAmount+order.ShippingCost+order.TotalAmount;
        }

        private static decimal CalculateDiscountAmount(Order order)
        {
            var discountedTotal = order.OrderItems.Sum(oi => oi.TotalPrice);
            order.DiscountAmount = order.TotalAmount - discountedTotal;

            // Handle edge case where discount equals total (likely means no discount)
            if (order.DiscountAmount == order.TotalAmount)
            {
                order.DiscountAmount = 0;
            }
            return order.DiscountAmount;
        }

        private async Task ProcessCheckoutTransactionAsync(Order order, Cart cart)
        {
            // Save order
            await unitOfWork.Orders.InsertAsync(order);

            // Clear cart
            cart.CartItems.Clear();
            unitOfWork.Carts.Update(cart);

            // Commit transaction
            await unitOfWork.CommitAsync();
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
            var order = await unitOfWork.Orders.FirstOrDefaultAsync(o => o.TrackingNumber == request.TrackingNumber);
            if (order == null)
            {
                return NotFound<CancelOrderResponse>("Order not found");
            }
            // check if it valid to cancel it 
            if (order.Status == OrderStatus.Shipped)
            {
                return BadRequest<CancelOrderResponse>("The order already shipped");
            }
            else
            {
                order.Status = OrderStatus.Cancelled;
            }
            unitOfWork.Orders.Update(order);
            await unitOfWork.CommitAsync();
            var ordMap = order.Adapt<CancelOrderResponse>();
            return Success(ordMap);
        }

        public async Task<ApiResponsePaginated<List<GetAllOrdersPaginatedResponse>>> GetAllOrdersPaginatedByAdminAsync(GetAllOrdersPaginatedRequest request)
        {
            await DoValidationAsync<GetAllOrdersPaginatedRequestValidator, GetAllOrdersPaginatedRequest>(request);
            var orders = await unitOfWork.Orders.GetAllAsync((request.PageNumber - 1) * request.PageSize, request.PageSize, default, o => o.OrderItems, o => o.User)
                .Filter(request)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            var totalCount = await unitOfWork.Orders.CountAsync();

            var ordsMap = orders.Adapt<List<GetAllOrdersPaginatedResponse>>();
            return Success(ordsMap, totalCount, request.PageNumber, request.PageSize);

        }

        public async Task<ApiResponsePaginated<List<GetAllUserOrdersPaginatedResponse>>> GetAllUserOrdersPaginatedAsync(GetAllUserOrdersPaginatedRequest request)
        {
            await DoValidationAsync<GetAllUserOrdersPaginatedRequestValidator, GetAllUserOrdersPaginatedRequest>(request);
            var orders = await unitOfWork.Orders.GetAllAsyncByEx(o => o.UserId == request.UserId, (request.PageNumber - 1) * request.PageSize, request.PageSize, default, o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            var totalCount = await unitOfWork.Orders.CountAsync();

            var ordsMap = orders.Adapt<List<GetAllUserOrdersPaginatedResponse>>();
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
 );
            var order = await query.FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound<GetOrderByAdminByTrackingNumberResponse>("Order not found");
            }
            var ordMap = new GetOrderByAdminByTrackingNumberResponse(
     order.Id,
     order.User?.UserName ?? string.Empty, // keep nullable-safe
     order.PaymentMethod,
     order.PaymentStatus,
     order.Governorate,
     order.City,
     order.Status,
     order.TrackingNumber,
     order.TotalAmount,
     order.ShippingCost,
     order.TaxAmount,
     order.DiscountAmount,
     order.FinalAmount,
     order.OrderDate,
     order.UpdatedAt,
     order.OrderItems
         .Where(oi => oi.Book != null)
         .Select(oi => new BookDTO(
             oi.Book.Title,
            (oi.UnitPrice) 
         ))
         .ToList()
 );
            return Success(ordMap);

        }
        // need to some edits to get user first and then search in thier orders
        public async Task<ApiResponse<GetUserOrderByTrackingNumberResponse>> GetOrderByTrackingNumberByUserAsync(GetUserOrderByTrackingNumberRequest request)
        {
            await DoValidationAsync<GetUserOrderByTrackingNumberRequestValidator, GetUserOrderByTrackingNumberRequest>(request);
            var order = await unitOfWork.Orders.FirstOrDefaultAsync(o => o.TrackingNumber == request.TrackingNumber, default, o => o.OrderItems, o => o.User);
            if (order == null)
            {
                return NotFound<GetUserOrderByTrackingNumberResponse>("Order not found");
            }
            var ordMap= new GetUserOrderByTrackingNumberResponse(
                order.Id,
                order.User?.UserName ?? string.Empty, 
                order.PaymentMethod,          
                order.PaymentStatus,          
                order.Governorate,                   
                order.City,                          
                order.Status,                         
                order.TrackingNumber,
                order.FinalAmount,                    
                order.OrderDate,
                order.OrderItems
                    .Where(oi => oi.Book != null)
                    .Select(oi => new BookDTO(
                        oi.Book.Title,
                        oi.UnitPrice
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
                o => o.User);
                
            if (order == null)
            {
                return NotFound<UpdateOrderStatusResponse>("Order not found");
            }

            // Validate status transition
            if (!IsValidStatusTransition(order.Status, request.NewStatus))
            {
                return BadRequest<UpdateOrderStatusResponse>($"Cannot change status from {order.Status} to {request.NewStatus}");
            }

            var oldStatus = order.Status;
            order.Status = request.NewStatus;
            
            // For COD orders, update payment status when order is delivered
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
                }
            }
            
            unitOfWork.Orders.Update(order);
            await unitOfWork.CommitAsync();

            var response = new UpdateOrderStatusResponse(
                order.Id,
                order.Status,
                order.TrackingNumber,
                DateTime.UtcNow
            );

            return Success(response);
        }

        private static bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            // Allow any status change for admin flexibility, but log invalid transitions
            // You can implement more restrictive rules here if needed
            return true;
        }
    }
}
