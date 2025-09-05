using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Base;
using Microsoft.Extensions.Logging;

namespace AseerAlkotb.Application.Services
{
    /// <summary>
    /// خدمة مزامنة حالات الطلبات والمدفوعات
    /// Service for synchronizing Order and Payment status
    /// </summary>
    public class OrderPaymentSyncService : IOrderPaymentSyncService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderPaymentSyncService> _logger;

        public OrderPaymentSyncService(
            IUnitOfWork unitOfWork,
            ILogger<OrderPaymentSyncService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// مزامنة حالة الدفع بناءً على تغيير حالة الطلب (للدفع عند الاستلام خاصة)
        /// Sync payment status based on order status change (especially for COD)
        /// </summary>
        public async Task<bool> SyncPaymentStatusFromOrderAsync(int orderId, OrderStatus newOrderStatus, OrderStatus previousOrderStatus)
        {
            try
            {
                _logger.LogInformation("Starting payment sync for Order {OrderId}: {PreviousStatus} → {NewStatus}", 
                    orderId, previousOrderStatus, newOrderStatus);

                var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                {
                    _logger.LogError("Order {OrderId} not found for payment sync", orderId);
                    return false;
                }

                var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
                if (payment == null)
                {
                    _logger.LogError("Payment not found for Order {OrderId}", orderId);
                    return false;
                }

                var newPaymentStatus = DeterminePaymentStatusFromOrder(newOrderStatus, order.PaymentMethod);
                if (newPaymentStatus == null)
                {
                    _logger.LogInformation("No payment status change required for Order {OrderId}", orderId);
                    return true; // No change needed
                }

                // Validate the transition is allowed
                if (!IsValidPaymentStatusTransition(payment.Status, newPaymentStatus.Value))
                {
                    _logger.LogWarning("Invalid payment status transition for Order {OrderId}: {CurrentStatus} → {NewStatus}", 
                        orderId, payment.Status, newPaymentStatus.Value);
                    return false;
                }

                // Apply the change
                payment.Status = newPaymentStatus.Value;
                order.PaymentStatus = newPaymentStatus.Value;

                _unitOfWork.Payments.Update(payment);
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Payment status synced for Order {OrderId}: Payment status set to {PaymentStatus}", 
                    orderId, newPaymentStatus.Value);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing payment status for Order {OrderId}", orderId);
                return false;
            }
        }

        /// <summary>
        /// مزامنة حالة الطلب بناءً على تغيير حالة الدفع (للدفع الإلكتروني)
        /// Sync order status based on payment status change (for online payments)
        /// </summary>
        public async Task<OrderStatus?> SyncOrderStatusFromPaymentAsync(int orderId, PaymentStatus newPaymentStatus, PaymentStatus previousPaymentStatus)
        {
            try
            {
                _logger.LogInformation("Starting order sync for Order {OrderId}: Payment {PreviousStatus} → {NewStatus}", 
                    orderId, previousPaymentStatus, newPaymentStatus);

                var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                {
                    _logger.LogError("Order {OrderId} not found for order sync", orderId);
                    return null;
                }

                // Only suggest changes for online payments, not COD
                if (order.PaymentMethod == PaymentMethod.CashOnDelivery)
                {
                    _logger.LogInformation("Skipping order sync for COD Order {OrderId}", orderId);
                    return null;
                }

                var suggestedOrderStatus = DetermineOrderStatusFromPayment(newPaymentStatus, order.Status);
                if (suggestedOrderStatus == null)
                {
                    _logger.LogInformation("No order status change suggested for Order {OrderId}", orderId);
                    return null;
                }

                // Validate the transition is allowed
                if (!IsValidOrderStatusTransition(order.Status, suggestedOrderStatus.Value))
                {
                    _logger.LogWarning("Invalid order status transition suggested for Order {OrderId}: {CurrentStatus} → {SuggestedStatus}", 
                        orderId, order.Status, suggestedOrderStatus.Value);
                    return null;
                }

                _logger.LogInformation("Suggested order status change for Order {OrderId}: {CurrentStatus} → {SuggestedStatus}", 
                    orderId, order.Status, suggestedOrderStatus.Value);

                return suggestedOrderStatus.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing order status for Order {OrderId}", orderId);
                return null;
            }
        }

        /// <summary>
        /// التحقق من تطابق حالات الطلب والدفع
        /// Validate consistency between order and payment status
        /// </summary>
        public async Task<(bool IsConsistent, string? InconsistencyReason)> ValidateStatusConsistencyAsync(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                {
                    return (false, $"Order {orderId} not found");
                }

                var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
                if (payment == null)
                {
                    return (false, $"Payment not found for Order {orderId}");
                }

                // Check if statuses are consistent based on business rules
                var inconsistency = CheckStatusInconsistency(order.Status, payment.Status, order.PaymentMethod);
                
                return string.IsNullOrEmpty(inconsistency) 
                    ? (true, null) 
                    : (false, inconsistency);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating status consistency for Order {OrderId}", orderId);
                return (false, $"Error validating consistency: {ex.Message}");
            }
        }

        /// <summary>
        /// إصلاح عدم تطابق الحالات تلقائياً
        /// Automatically fix status inconsistencies
        /// </summary>
        public async Task<bool> AutoFixStatusInconsistencyAsync(int orderId)
        {
            try
            {
                var (isConsistent, reason) = await ValidateStatusConsistencyAsync(orderId);
                if (isConsistent)
                {
                    _logger.LogInformation("Order {OrderId} status is already consistent", orderId);
                    return true;
                }

                _logger.LogInformation("Attempting to fix status inconsistency for Order {OrderId}: {Reason}", orderId, reason);

                var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
                var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);

                if (order == null || payment == null)
                {
                    return false;
                }

                // Apply business logic to fix inconsistency
                bool wasFixed = false;

                // For COD orders, payment status should follow order status
                if (order.PaymentMethod == PaymentMethod.CashOnDelivery)
                {
                    var correctPaymentStatus = DeterminePaymentStatusFromOrder(order.Status, PaymentMethod.CashOnDelivery);
                    if (correctPaymentStatus.HasValue && payment.Status != correctPaymentStatus.Value)
                    {
                        payment.Status = correctPaymentStatus.Value;
                        order.PaymentStatus = correctPaymentStatus.Value;
                        wasFixed = true;
                    }
                }
                // For online payments, order status might need adjustment based on payment
                else
                {
                    var suggestedOrderStatus = DetermineOrderStatusFromPayment(payment.Status, order.Status);
                    if (suggestedOrderStatus.HasValue && order.Status != suggestedOrderStatus.Value)
                    {
                        order.Status = suggestedOrderStatus.Value;
                        wasFixed = true;
                    }
                }

                if (wasFixed)
                {
                    _unitOfWork.Orders.Update(order);
                    _unitOfWork.Payments.Update(payment);
                    await _unitOfWork.CommitAsync();

                    _logger.LogInformation("Status inconsistency fixed for Order {OrderId}", orderId);
                }

                return wasFixed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error auto-fixing status inconsistency for Order {OrderId}", orderId);
                return false;
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// تحديد حالة الدفع المناسبة بناءً على حالة الطلب
        /// Determine appropriate payment status based on order status
        /// </summary>
        private PaymentStatus? DeterminePaymentStatusFromOrder(OrderStatus orderStatus, PaymentMethod paymentMethod)
        {
            // Rules apply mainly to COD orders
            if (paymentMethod == PaymentMethod.CashOnDelivery)
            {
                return orderStatus switch
                {
                    OrderStatus.Delivered => PaymentStatus.Paid,
                    OrderStatus.Cancelled => PaymentStatus.Cancelled,
                    _ => null // No change needed for other statuses
                };
            }

            // For online payments, payment status drives order status, not the other way around
            return null;
        }

        /// <summary>
        /// تحديد حالة الطلب المقترحة بناءً على حالة الدفع
        /// Determine suggested order status based on payment status
        /// </summary>
        private OrderStatus? DetermineOrderStatusFromPayment(PaymentStatus paymentStatus, OrderStatus currentOrderStatus)
        {
            return paymentStatus switch
            {
                PaymentStatus.Paid when currentOrderStatus == OrderStatus.Pending => OrderStatus.Approved,
                PaymentStatus.Failed when currentOrderStatus == OrderStatus.Pending => OrderStatus.Cancelled,
                PaymentStatus.Cancelled when currentOrderStatus == OrderStatus.Pending => OrderStatus.Cancelled,
                _ => null // No change suggested
            };
        }

        /// <summary>
        /// التحقق من صحة انتقال حالة الدفع
        /// Validate payment status transition
        /// </summary>
        private bool IsValidPaymentStatusTransition(PaymentStatus current, PaymentStatus target)
        {
            // Define allowed transitions
            return (current, target) switch
            {
                (PaymentStatus.Pending, PaymentStatus.Processing) => true,
                (PaymentStatus.Pending, PaymentStatus.Paid) => true,
                (PaymentStatus.Pending, PaymentStatus.Failed) => true,
                (PaymentStatus.Pending, PaymentStatus.Cancelled) => true,
                (PaymentStatus.Processing, PaymentStatus.Paid) => true,
                (PaymentStatus.Processing, PaymentStatus.Failed) => true,
                (PaymentStatus.Paid, PaymentStatus.Refunded) => true,
                (PaymentStatus.Paid, PaymentStatus.PartiallyRefunded) => true,
                _ when current == target => true, // Same status is always valid
                _ => false
            };
        }

        /// <summary>
        /// التحقق من صحة انتقال حالة الطلب
        /// Validate order status transition
        /// </summary>
        private bool IsValidOrderStatusTransition(OrderStatus current, OrderStatus target)
        {
            // Define allowed transitions
            return (current, target) switch
            {
                (OrderStatus.Pending, OrderStatus.Approved) => true,
                (OrderStatus.Pending, OrderStatus.Cancelled) => true,
                (OrderStatus.Approved, OrderStatus.Shipped) => true,
                (OrderStatus.Approved, OrderStatus.Cancelled) => true,
                (OrderStatus.Shipped, OrderStatus.Delivered) => true,
                (OrderStatus.Shipped, OrderStatus.Cancelled) => true,
                _ when current == target => true, // Same status is always valid
                _ => false
            };
        }

        /// <summary>
        /// فحص عدم تطابق الحالات
        /// Check for status inconsistency
        /// </summary>
        private string? CheckStatusInconsistency(OrderStatus orderStatus, PaymentStatus paymentStatus, PaymentMethod paymentMethod)
        {
            if (paymentMethod == PaymentMethod.CashOnDelivery)
            {
                return (orderStatus, paymentStatus) switch
                {
                    (OrderStatus.Delivered, var ps) when ps != PaymentStatus.Paid => 
                        $"COD order is delivered but payment is {ps} instead of Paid",
                    
                    (OrderStatus.Cancelled, var ps) when ps != PaymentStatus.Cancelled && ps != PaymentStatus.Failed => 
                        $"COD order is cancelled but payment is {ps} instead of Cancelled",
                    
                    _ => null
                };
            }
            else
            {
                return (orderStatus, paymentStatus) switch
                {
                    (OrderStatus.Pending, PaymentStatus.Paid) => 
                        "Online payment is paid but order is still pending",
                    
                    (OrderStatus.Approved, PaymentStatus.Failed) => 
                        "Order is approved but payment failed",
                    
                    (OrderStatus.Approved, PaymentStatus.Cancelled) => 
                        "Order is approved but payment is cancelled",
                    
                    _ => null
                };
            }
        }

        #endregion
    }
}