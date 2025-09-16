using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Contracts.External;
using AseerAlkotb.Domain.Interfaces.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AseerAlkotb.Application.Services
{
    public class ExpiredCartCleanupService : BackgroundService, IExpiredCartCleanupService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExpiredCartCleanupService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromDays(1); // Check every day
        private readonly TimeSpan _cartExpirationTime = TimeSpan.FromDays(3); // Carts expire after 3 days

        public ExpiredCartCleanupService(
            IServiceProvider serviceProvider,
            ILogger<ExpiredCartCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Expired Cart Cleanup Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanExpiredCartsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning expired carts.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Expired Cart Cleanup Service is stopping.");
        }

        public async Task CleanExpiredCartsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var expirationTime = DateTime.UtcNow.Subtract(_cartExpirationTime);
            var expiredCarts = await unitOfWork.Carts.GetCartsOlderThanAsync(expirationTime);

            _logger.LogInformation($"Found {expiredCarts.Count} expired carts to clean up.");

            foreach (var cart in expiredCarts)
            {
                try
                {
                    foreach (var item in cart.CartItems.ToList())
                    {
                        item.Book.StockQuantity += item.Quantity;
                        await unitOfWork.Carts.RemoveCartItemAsync(item);
                    }

                    cart.UpdatedAt = DateTime.UtcNow;
                    unitOfWork.Carts.Update(cart);

                    _logger.LogInformation($"Cleared expired cart {cart.Id} for user {cart.UserId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error clearing cart {cart.Id}");
                }
            }

            await unitOfWork.CommitAsync();
        }
    }
}

