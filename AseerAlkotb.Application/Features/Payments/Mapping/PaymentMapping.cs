using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Domain.Entites;
using Mapster;

namespace AseerAlkotb.Application.Features.Payments.Mapping
{
    public class PaymentMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Payment to GetAllPaymentsPaginatedResponse - Manual mapping
            config.NewConfig<Payment, GetAllPaymentsPaginatedResponse>()
                .ConstructUsing(src => new GetAllPaymentsPaginatedResponse(
                    src.Id,
                    src.TransactionId ?? string.Empty,
                    src.OrderId,
                    GetCustomerName(src),
                    GetCustomerEmail(src),
                    src.Method,
                    src.Status,
                    src.Amount,
                    src.Currency ?? "EGP",
                    src.PaymentDate,
                    src.PaymobOrderId
                ));

            // Payment to GetPaymentByIdResponse - Manual mapping
            config.NewConfig<Payment, GetPaymentByIdResponse>()
                .ConstructUsing(src => new GetPaymentByIdResponse(
                    src.Id,
                    src.TransactionId ?? string.Empty,
                    src.OrderId,
                    src.UserId,
                    GetCustomerName(src),
                    GetCustomerEmail(src),
                    GetCustomerPhone(src),
                    src.Method,
                    src.Status,
                    src.Amount,
                    src.Currency ?? "EGP",
                    src.PaymentDate,
                    src.PaymobOrderId,
                    null, // AdminNotes - not currently used
                    src.ProviderPayload // ProviderPayload
                ));
        }

        /// <summary>
        /// Safely gets customer name with null checking
        /// </summary>
        private static string GetCustomerName(Payment payment)
        {
            if (payment?.User == null)
                return "Unknown Customer";

            var firstName = payment.User.FirstName?.Trim() ?? string.Empty;
            var lastName = payment.User.LastName?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
                return "Unknown Customer";

            return $"{firstName} {lastName}".Trim();
        }

        /// <summary>
        /// Safely gets customer email with null checking
        /// </summary>
        private static string GetCustomerEmail(Payment payment)
        {
            return payment?.User?.Email?.Trim() ?? "N/A";
        }

        /// <summary>
        /// Safely gets customer phone with null checking
        /// </summary>
        private static string GetCustomerPhone(Payment payment)
        {
            return payment?.User?.PhoneNumber?.Trim() ?? "N/A";
        }
    }

    /// <summary>
    /// Manual mapping extension methods for Payment entity
    /// </summary>
    public static class PaymentMappingExtensions
    {
        public static GetAllPaymentsPaginatedResponse ToGetAllPaymentsPaginatedResponse(this Payment payment)
        {
            return new GetAllPaymentsPaginatedResponse(
                payment.Id,
                payment.TransactionId ?? string.Empty,
                payment.OrderId,
                GetCustomerName(payment),
                GetCustomerEmail(payment),
                payment.Method,
                payment.Status,
                payment.Amount,
                payment.Currency ?? "EGP",
                payment.PaymentDate,
                payment.PaymobOrderId
            );
        }

        public static GetPaymentByIdResponse ToGetPaymentByIdResponse(this Payment payment)
        {
            return new GetPaymentByIdResponse(
                payment.Id,
                payment.TransactionId ?? string.Empty,
                payment.OrderId,
                payment.UserId,
                GetCustomerName(payment),
                GetCustomerEmail(payment),
                GetCustomerPhone(payment),
                payment.Method,
                payment.Status,
                payment.Amount,
                payment.Currency ?? "EGP",
                payment.PaymentDate,
                payment.PaymobOrderId,
                null, // AdminNotes - not currently used
                payment.ProviderPayload // ProviderPayload
            );
        }

        public static List<GetAllPaymentsPaginatedResponse> ToGetAllPaymentsPaginatedResponseList(this IEnumerable<Payment> payments)
        {
            return payments.Select(p => p.ToGetAllPaymentsPaginatedResponse()).ToList();
        }

        /// <summary>
        /// Safely gets customer name with null checking
        /// </summary>
        private static string GetCustomerName(Payment payment)
        {
            if (payment?.User == null)
                return "Unknown Customer";

            var firstName = payment.User.FirstName?.Trim() ?? string.Empty;
            var lastName = payment.User.LastName?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
                return "Unknown Customer";

            return $"{firstName} {lastName}".Trim();
        }

        /// <summary>
        /// Safely gets customer email with null checking
        /// </summary>
        private static string GetCustomerEmail(Payment payment)
        {
            return payment?.User?.Email?.Trim() ?? "N/A";
        }

        /// <summary>
        /// Safely gets customer phone with null checking
        /// </summary>
        private static string GetCustomerPhone(Payment payment)
        {
            return payment?.User?.PhoneNumber?.Trim() ?? "N/A";
        }
    }
}