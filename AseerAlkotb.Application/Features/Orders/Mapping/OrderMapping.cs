

using AseerAlkotb.Application.Features.Books.DTOs;
using AseerAlkotb.Application.Features.OrderItems.DTOs;
using AseerAlkotb.Application.Features.Orders.Requests;
using AseerAlkotb.Application.Features.Orders.Responses;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using Mapster;

namespace AseerAlkotb.Application.Features.Orders.Mapping
{
        public class OrderMapping : IRegister
        {
            public void Register(TypeAdapterConfig config)
            {
            // Add Order Mapping (Request → Entity) WITHOUT OrderItems (they come from cart in service)
            config.NewConfig<AddOrderRequest, Order>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.ShippingCost)
                .Ignore(dest => dest.TaxAmount)
                .Ignore(dest => dest.DiscountAmount)
                .Ignore(dest => dest.TrackingNumber)
                .Ignore(dest => dest.OrderItems) // Important: Will be set in service
                .Map(dest => dest.Status, src => OrderStatus.Pending)
                .Map(dest => dest.OrderDate, src => DateTime.Now)
                .Map(dest => dest.PaymentStatus, src => PaymentStatus.Pending);

            // OrderItem Mapping (Book → OrderItem)
            config.NewConfig<Book, OrderItem>()
                .Ignore(dest => dest.Order)
                .Map(dest => dest.UnitPrice, src => src.Price) // Always from current Book price
                .Map(dest => dest.Quantity, _ => 1); // Quantity set in service based on cart

            // Add Order Response (Entity → Response)
            config.NewConfig<Order, AddOrderResponse>()
                    .Map(dest => dest.Id, src => src.Id);

                config.NewConfig<Order, CancelOrderResponse>()
                    .Map(dest => dest.Id, src => src.Id);

                // Get Order By User (Entity → Response)
                config.NewConfig<Order, GetUserOrderByTrackingNumberResponse>()
                    .Map(dest => dest.UserName, src => src.User.FirstName)
                    .Map(dest => dest.OrderStatus, src => src.Status)
                    .Map(dest => dest.Books, src => src.OrderItems
                        .Select(oi => new BookDTO(oi.Book.Title, (int)oi.UnitPrice))
                        .ToList());

                // Get Order By Admin (Entity → Response)
                config.NewConfig<Order, GetOrderByAdminByTrackingNumberResponse>()
                    .Map(dest => dest.UserName, src => src.User.FirstName)
                    .Map(dest => dest.OrderStatus, src => src.Status)
                    .Map(dest => dest.Books, src => src.OrderItems
                        .Select(oi => new BookDTO(oi.Book.Title, (int)oi.UnitPrice))
                        .ToList());

                // Get All User Orders Paginated
                config.NewConfig<Order, GetAllUserOrdersPaginatedResponse>()
                    .Map(dest => dest.UserName, src => src.User.FirstName)
                    .Map(dest => dest.OrderStatus, src => src.Status)
                    .Map(dest => dest.Books, src => src.OrderItems
                        .Select(oi => new BookDTO(oi.Book.Title, (int)oi.UnitPrice))
                        .ToList());

                // Get All Orders Paginated
                config.NewConfig<Order, GetAllOrdersPaginatedResponse>()
                    .Map(dest => dest.UserName, src => src.User.FirstName)
                    .Map(dest => dest.OrderStatus, src => src.Status)
                    .Map(dest => dest.Books, src => src.OrderItems
                        .Select(oi => new BookDTO(oi.Book.Title, (int)oi.UnitPrice))
                        .ToList());
            }
        }
}
