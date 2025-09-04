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
            return ((int)method) switch
            {
                1 => "fas fa-money-bill-wave", // CashOnDelivery
                2 => "fas fa-credit-card", // Card
                3 => "fas fa-mobile-alt", // Wallet
                0 => "fas fa-question-circle", // Handle 0 value in database
                _ => "fas fa-question-circle"
            };
        }

        public static string GetPaymentMethodDisplayName(PaymentMethod method)
        {
            return ((int)method) switch
            {
                1 => "Cash on Delivery", // CashOnDelivery
                2 => "Credit/Debit Card", // Card
                3 => "Mobile Wallet", // Wallet
                0 => "Unknown Method", // Handle 0 value in database
                _ => $"Method {(int)method}"
            };
        }

        public static string GetPaymentStatusBadgeClass(PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Pending => "bg-warning",
                PaymentStatus.Processing => "bg-info",
                PaymentStatus.Paid => "bg-success",
                PaymentStatus.Failed => "bg-danger",
                PaymentStatus.Cancelled => "bg-secondary",
                PaymentStatus.Refunded => "bg-dark",
                PaymentStatus.PartiallyRefunded => "bg-primary",
                _ => "bg-secondary"
            };
        }
        
        public static string GetPaymentStatusIcon(PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Pending => "fas fa-clock",
                PaymentStatus.Processing => "fas fa-spinner fa-spin",
                PaymentStatus.Paid => "fas fa-check-circle",
                PaymentStatus.Failed => "fas fa-times-circle",
                PaymentStatus.Cancelled => "fas fa-ban",
                PaymentStatus.Refunded => "fas fa-undo",
                PaymentStatus.PartiallyRefunded => "fas fa-undo-alt",
                _ => "fas fa-question-circle"
            };
        }
    }
}