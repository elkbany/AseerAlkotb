using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Dashboard.Helpers
{
    public static class ViewHelper
    {
        public static string GetStatusBadgeClass(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "bg-warning",
                OrderStatus.Approved => "bg-info",
                OrderStatus.Shipped => "bg-primary",
                OrderStatus.Delivered => "bg-success",
                OrderStatus.Cancelled => "bg-danger",
                _ => "bg-secondary"
            };
        }

        public static string GetStatusIcon(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "fas fa-clock",
                OrderStatus.Approved => "fas fa-check",
                OrderStatus.Shipped => "fas fa-shipping-fast",
                OrderStatus.Delivered => "fas fa-check-circle",
                OrderStatus.Cancelled => "fas fa-times-circle",
                _ => "fas fa-question-circle"
            };
        }

        public static string GetPaymentMethodIcon(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.CashOnDelivery => "fas fa-money-bill-wave",
                PaymentMethod.Card => "fas fa-credit-card",
                PaymentMethod.MobileWallet => "fas fa-mobile-alt",
                _ => "fas fa-question-circle"
            };
        }

        public static string GetPaymentMethodDisplayName(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.CashOnDelivery => "Cash on Delivery",
                PaymentMethod.Card => "Credit/Debit Card",
                PaymentMethod.MobileWallet => "Mobile Wallet",
                _ => method.ToString()
            };
        }

        public static string GetPaymentStatusBadgeClass(PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Pending => "bg-warning",
                PaymentStatus.Paid => "bg-success",
                PaymentStatus.Failed => "bg-danger",
                PaymentStatus.Cancelled => "bg-secondary",
                _ => "bg-secondary"
            };
        }
    }
}