using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Contracts
{
    /// <summary>
    /// خدمة مزامنة حالات الطلبات والمدفوعات
    /// Service for synchronizing Order and Payment status
    /// </summary>
    public interface IOrderPaymentSyncService
    {
        /// <summary>
        /// مزامنة حالة الدفع بناءً على تغيير حالة الطلب
        /// Sync payment status based on order status change
        /// </summary>
        /// <param name="orderId">معرف الطلب - Order ID</param>
        /// <param name="newOrderStatus">حالة الطلب الجديدة - New order status</param>
        /// <param name="previousOrderStatus">حالة الطلب السابقة - Previous order status</param>
        /// <returns>نجح التزامن أم لا - Success status</returns>
        Task<bool> SyncPaymentStatusFromOrderAsync(int orderId, OrderStatus newOrderStatus, OrderStatus previousOrderStatus);

        /// <summary>
        /// مزامنة حالة الطلب بناءً على تغيير حالة الدفع
        /// Sync order status based on payment status change
        /// </summary>
        /// <param name="orderId">معرف الطلب - Order ID</param>
        /// <param name="newPaymentStatus">حالة الدفع الجديدة - New payment status</param>
        /// <param name="previousPaymentStatus">حالة الدفع السابقة - Previous payment status</param>
        /// <returns>حالة الطلب المقترحة - Suggested order status (null if no change needed)</returns>
        Task<OrderStatus?> SyncOrderStatusFromPaymentAsync(int orderId, PaymentStatus newPaymentStatus, PaymentStatus previousPaymentStatus);

        /// <summary>
        /// التحقق من تطابق حالات الطلب والدفع
        /// Validate consistency between order and payment status
        /// </summary>
        /// <param name="orderId">معرف الطلب - Order ID</param>
        /// <returns>معلومات عدم التطابق إن وجدت - Inconsistency details if found</returns>
        Task<(bool IsConsistent, string? InconsistencyReason)> ValidateStatusConsistencyAsync(int orderId);

        /// <summary>
        /// إصلاح عدم تطابق الحالات تلقائياً
        /// Automatically fix status inconsistencies
        /// </summary>
        /// <param name="orderId">معرف الطلب - Order ID</param>
        /// <returns>تم الإصلاح أم لا - Whether fix was applied</returns>
        Task<bool> AutoFixStatusInconsistencyAsync(int orderId);
    }
}