using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Domain.Entites;
using Mapster;

namespace AseerAlkotb.Application.Features.Payments.Mapping
{
    public class PaymentMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Payment to GetAllPaymentsPaginatedResponse
            config.NewConfig<Payment, GetAllPaymentsPaginatedResponse>()
                .Map(dest => dest.CustomerName, src => $"{src.User.FirstName} {src.User.LastName}")
                .Map(dest => dest.CustomerEmail, src => src.User.Email)
                .Map(dest => dest.PaymentMethod, src => src.Method)
                .Map(dest => dest.Status, src => src.Status);

            // Payment to GetPaymentByIdResponse
            config.NewConfig<Payment, GetPaymentByIdResponse>()
                .Map(dest => dest.CustomerName, src => $"{src.User.FirstName} {src.User.LastName}")
                .Map(dest => dest.CustomerEmail, src => src.User.Email)
                .Map(dest => dest.CustomerPhone, src => src.User.PhoneNumber)
                .Map(dest => dest.PaymentMethod, src => src.Method)
                .Map(dest => dest.Status, src => src.Status);
        }
    }
}