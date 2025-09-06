using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Base;
using Microsoft.Extensions.Logging;

namespace AseerAlkotb.Application.Services
{
    /// <summary>
    /// خدمة إعادة محاولة المدفوعات الفاشلة
    /// Service for retrying failed payments with intelligent retry logic
    /// </summary>
    public class PaymentRetryService : IPaymentRetryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentRetryService> _logger;
        
        // إعدادات إعادة المحاولة - Retry configuration
        private const int MaxRetryAttempts = 3;
        private const int RetryWindowHours = 24;
        private static readonly TimeSpan[] RetryDelays = { 
            TimeSpan.FromSeconds(2),   // أول محاولة - First retry
            TimeSpan.FromSeconds(5),   // ثاني محاولة - Second retry  
            TimeSpan.FromSeconds(10)   // ثالث محاولة - Third retry
        };

        public PaymentRetryService(
            IUnitOfWork unitOfWork,
            IPaymentService paymentService,
            ILogger<PaymentRetryService> logger)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// إعادة محاولة دفع معين
        /// Retry a specific payment
        /// </summary>
        public async Task<bool> RetryPaymentAsync(int paymentId)
        {
            try
            {
                _logger.LogInformation("Starting payment retry for Payment {PaymentId}", paymentId);

                var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);
                if (payment == null)
                {
                    _logger.LogError("Payment {PaymentId} not found", paymentId);
                    return false;
                }

                var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == payment.OrderId);
                if (order == null)
                {
                    _logger.LogError("Order {OrderId} not found for Payment {PaymentId}", payment.OrderId, paymentId);
                    return false;
                }

                // التحقق من الأهلية - Check eligibility
                var (isEligible, reason) = await IsPaymentEligibleForRetryAsync(paymentId);
                if (!isEligible)
                {
                    _logger.LogWarning("Payment {PaymentId} is not eligible for retry: {Reason}", paymentId, reason);
                    return false;
                }

                // حساب التأخير قبل المحاولة - Calculate delay before retry
                var retryAttempt = await GetCurrentRetryAttemptAsync(paymentId);
                if (retryAttempt > 0 && retryAttempt <= RetryDelays.Length)
                {
                    var delay = RetryDelays[retryAttempt - 1];
                    _logger.LogInformation("Waiting {DelaySeconds} seconds before retry attempt {Attempt} for Payment {PaymentId}", 
                        delay.TotalSeconds, retryAttempt, paymentId);
                    await Task.Delay(delay);
                }

                // محاولة إعادة معالجة الدفع - Attempt to reprocess payment
                bool retrySuccess = false;
                
                if (order.PaymentMethod == PaymentMethod.CashOnDelivery)
                {
                    // للدفع عند الاستلام، فقط إعادة تعيين الحالة - For COD, just reset status
                    payment.Status = PaymentStatus.Pending;
                    order.PaymentStatus = PaymentStatus.Pending;
                    retrySuccess = true;
                }
                else
                {
                    // للدفع الإلكتروني، إعادة تهيئة مع بوابة الدفع - For online payments, reinitialize with gateway
                    try
                    {
                        // هنا يمكن إضافة منطق إعادة تهيئة بوابة الدفع
                        // Here we can add payment gateway reinitialization logic
                        payment.Status = PaymentStatus.Pending;
                        order.PaymentStatus = PaymentStatus.Pending;
                        retrySuccess = true;
                        
                        _logger.LogInformation("Payment gateway reinitialization would be triggered for Payment {PaymentId}", paymentId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to reinitialize payment gateway for Payment {PaymentId}", paymentId);
                        retrySuccess = false;
                    }
                }

                // تسجيل محاولة إعادة المحاولة - Record retry attempt
                await RecordRetryAttemptAsync(paymentId, retrySuccess);

                if (retrySuccess)
                {
                    _unitOfWork.Payments.Update(payment);
                    _unitOfWork.Orders.Update(order);
                    await _unitOfWork.CommitAsync();

                    _logger.LogInformation("Payment retry successful for Payment {PaymentId}", paymentId);
                }
                else
                {
                    _logger.LogError("Payment retry failed for Payment {PaymentId}", paymentId);
                }

                return retrySuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during payment retry for Payment {PaymentId}", paymentId);
                return false;
            }
        }

        /// <summary>
        /// إعادة محاولة دفع بناءً على معرف الطلب
        /// Retry payment by order ID
        /// </summary>
        public async Task<bool> RetryPaymentByOrderAsync(int orderId)
        {
            try
            {
                var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
                if (payment == null)
                {
                    _logger.LogError("No payment found for Order {OrderId}", orderId);
                    return false;
                }

                return await RetryPaymentAsync(payment.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrying payment for Order {OrderId}", orderId);
                return false;
            }
        }

        /// <summary>
        /// معالجة جميع المدفوعات الفاشلة المؤهلة لإعادة المحاولة
        /// Process all failed payments eligible for retry
        /// </summary>
        public async Task<int> ProcessFailedPaymentsAsync()
        {
            try
            {
                _logger.LogInformation("Starting batch processing of failed payments");

                // جلب المدفوعات الفاشلة في آخر 24 ساعة - Get failed payments in last 24 hours
                var cutoffDate = DateTime.UtcNow.AddHours(-RetryWindowHours);
                
                var failedPayments = await _unitOfWork.Payments.GetAllAsync(
                    p => (p.Status == PaymentStatus.Failed || p.Status == PaymentStatus.Cancelled) 
                         && p.PaymentDate >= cutoffDate
                );

                int processedCount = 0;
                int successCount = 0;

                foreach (var payment in failedPayments)
                {
                    var eligibility = await IsPaymentEligibleForRetryAsync(payment.Id);
                    if (!eligibility.IsEligible)
                    {
                        _logger.LogDebug("Skipping Payment {PaymentId}: {Reason}", payment.Id, eligibility.Reason);
                        continue;
                    }

                    processedCount++;
                    
                    var retryResult = await RetryPaymentAsync(payment.Id);
                    if (retryResult)
                    {
                        successCount++;
                    }

                    // تأخير قصير بين المحاولات لتجنب إرهاق النظام - Short delay between attempts to avoid system overload
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                }

                _logger.LogInformation("Batch processing completed: {ProcessedCount} payments processed, {SuccessCount} successful retries", 
                    processedCount, successCount);

                return processedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during batch processing of failed payments");
                return 0;
            }
        }

        /// <summary>
        /// التحقق من أهلية الدفع لإعادة المحاولة
        /// Check if payment is eligible for retry
        /// </summary>
        public async Task<(bool IsEligible, string Reason)> IsPaymentEligibleForRetryAsync(int paymentId)
        {
            try
            {
                var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);
                if (payment == null)
                {
                    return (false, "Payment not found");
                }

                var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == payment.OrderId);
                if (order == null)
                {
                    return (false, "Associated order not found");
                }

                // فحص حالة الدفع - Check payment status
                if (payment.Status != PaymentStatus.Failed && payment.Status != PaymentStatus.Cancelled)
                {
                    return (false, $"Payment status is {payment.Status}, only Failed or Cancelled payments can be retried");
                }

                // فحص حالة الطلب - Check order status
                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Approved)
                {
                    return (false, $"Order status is {order.Status}, only Pending or Approved orders are eligible for payment retry");
                }

                // فحص نافذة الوقت - Check time window
                var timeSincePayment = DateTime.UtcNow - payment.PaymentDate;
                if (timeSincePayment.TotalHours > RetryWindowHours)
                {
                    return (false, $"Payment is outside retry window (older than {RetryWindowHours} hours)");
                }

                // فحص عدد المحاولات - Check retry attempts
                var currentAttempts = await GetCurrentRetryAttemptAsync(paymentId);
                if (currentAttempts >= MaxRetryAttempts)
                {
                    return (false, $"Maximum retry attempts ({MaxRetryAttempts}) reached");
                }

                return (true, "Payment is eligible for retry");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking payment eligibility for Payment {PaymentId}", paymentId);
                return (false, $"Error checking eligibility: {ex.Message}");
            }
        }

        /// <summary>
        /// الحصول على عدد محاولات إعادة المحاولة المتبقية
        /// Get remaining retry attempts for a payment
        /// </summary>
        public async Task<int> GetRemainingRetryAttemptsAsync(int paymentId)
        {
            try
            {
                var currentAttempts = await GetCurrentRetryAttemptAsync(paymentId);
                return Math.Max(0, MaxRetryAttempts - currentAttempts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting remaining retry attempts for Payment {PaymentId}", paymentId);
                return 0;
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// الحصول على رقم المحاولة الحالية
        /// Get current retry attempt number
        /// </summary>
        private async Task<int> GetCurrentRetryAttemptAsync(int paymentId)
        {
            // هذا مجرد تمثيل بسيط - يمكن تحسينه بجدول منفصل لتتبع المحاولات
            // This is a simple representation - can be improved with a separate table for tracking attempts
            
            // للآن، نفترض أن كل دفع فاشل له محاولة واحدة
            // For now, assume each failed payment has one attempt
            var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);
            
            // يمكن إضافة جدول منفصل لتتبع محاولات إعادة المحاولة
            // Can add a separate table to track retry attempts
            return payment?.Status == PaymentStatus.Failed ? 1 : 0;
        }

        /// <summary>
        /// تسجيل محاولة إعادة المحاولة
        /// Record a retry attempt
        /// </summary>
        private async Task RecordRetryAttemptAsync(int paymentId, bool successful)
        {
            // يمكن إضافة جدول منفصل لتسجيل محاولات إعادة المحاولة
            // Can add a separate table to log retry attempts
            
            _logger.LogInformation("Retry attempt recorded for Payment {PaymentId}: Success = {Success}", 
                paymentId, successful);
            
            // إنتظار للتنفيذ في المستقبل
            // Awaiting future implementation
            await Task.CompletedTask;
        }

        #endregion
    }
}