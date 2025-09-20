using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Contracts.External;
using AseerAlkotb.Domain.Interfaces.Base;
using AseerAlkotb.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AseerAlkotb.Application.Services
{
    public class StalePaymentCleanupService : BackgroundService, IStalePaymentCleanupService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StalePaymentCleanupService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30); // Check every 30 minutes
        private readonly TimeSpan _paymentStaleTime = TimeSpan.FromHours(1); // Payments stale after 1 hour

        public StalePaymentCleanupService(
            IServiceProvider serviceProvider,
            ILogger<StalePaymentCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stale Payment Cleanup Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanStalePaymentsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning stale payments.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Stale Payment Cleanup Service is stopping.");
        }

        public async Task CleanStalePaymentsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var staleTime = DateTime.UtcNow.Subtract(_paymentStaleTime);
            var stalePayments = await unitOfWork.Payments.GetAllAsync(
                p => p.Status == PaymentStatus.Processing && p.PaymentDate < staleTime
            );

            _logger.LogInformation($"Found {stalePayments.Count} stale payments to clean up.");

            foreach (var payment in stalePayments)
            {
                try
                {
                    // Store the original status for logging
                    var originalStatus = payment.Status;
                    
                    // Update payment status to Failed
                    payment.Status = PaymentStatus.Failed;
                    payment.ProviderPayload += $" | Marked as stale at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
                    
                    unitOfWork.Payments.Update(payment);

                    // Also update the associated order if it exists
                    var order = await unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == payment.OrderId);
                    if (order != null)
                    {
                        var originalOrderStatus = order.PaymentStatus;
                        order.PaymentStatus = PaymentStatus.Failed;
                        unitOfWork.Orders.Update(order);
                        
                        _logger.LogInformation($"Updated order {order.Id} payment status from {originalOrderStatus} to {order.PaymentStatus}");
                    }

                    _logger.LogInformation($"Marked stale payment {payment.Id} (Transaction: {payment.TransactionId}) from {originalStatus} to {payment.Status}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating stale payment {payment.Id}");
                }
            }

            if (stalePayments.Any())
            {
                await unitOfWork.CommitAsync();
                _logger.LogInformation($"Committed changes for {stalePayments.Count} stale payments");
            }
        }
    }
}