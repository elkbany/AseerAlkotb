using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Payments.Requests
{
    public record InitializePaymentRequest(
        Order order
    );
}