using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Contracts
{
    /// <summary>
    /// خدمة إعادة محاولة المدفوعات الفاشلة
    /// Service for retrying failed payments
    /// </summary>
    public interface IPaymentRetryService
    {
        /// <summary>
        /// إعادة محاولة دفع معين
        /// Retry a specific payment
        /// </summary>
        /// <param name="paymentId">معرف الدفع - Payment ID</param>
        /// <returns>نجحت المحاولة أم لا - Success status</returns>
        Task<bool> RetryPaymentAsync(int paymentId);

        /// <summary>
        /// إعادة محاولة دفع بناءً على معرف الطلب
        /// Retry payment by order ID
        /// </summary>
        /// <param name="orderId">معرف الطلب - Order ID</param>
        /// <returns>نجحت المحاولة أم لا - Success status</returns>
        Task<bool> RetryPaymentByOrderAsync(int orderId);

        /// <summary>
        /// معالجة جميع المدفوعات الفاشلة المؤهلة لإعادة المحاولة
        /// Process all failed payments eligible for retry
        /// </summary>
        /// <returns>عدد المدفوعات المُعالجة - Number of payments processed</returns>
        Task<int> ProcessFailedPaymentsAsync();

        /// <summary>
        /// التحقق من أهلية الدفع لإعادة المحاولة
        /// Check if payment is eligible for retry
        /// </summary>
        /// <param name="paymentId">معرف الدفع - Payment ID</param>
        /// <returns>مؤهل أم لا مع السبب - Eligibility status with reason</returns>
        Task<(bool IsEligible, string Reason)> IsPaymentEligibleForRetryAsync(int paymentId);

        /// <summary>
        /// الحصول على عدد محاولات إعادة المحاولة المتبقية
        /// Get remaining retry attempts for a payment
        /// </summary>
        /// <param name="paymentId">معرف الدفع - Payment ID</param>
        /// <returns>عدد المحاولات المتبقية - Remaining attempts</returns>
        Task<int> GetRemainingRetryAttemptsAsync(int paymentId);
    }
}