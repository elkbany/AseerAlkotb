using AseerAlkotb.Domain.Entites;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Interfaces.Repositories
{
    public interface IPaymentRepository : IGenericRepository<Payment, int>
    {
        Task<Payment?> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);

        Task<List<Payment>> GetPaymentsByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        Task UpdatePaymentStatusAsync(int paymentId, string status, CancellationToken cancellationToken = default);

        Task<List<Payment>> GetPaymentsAsync(
            int? userId = null,
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            CancellationToken cancellationToken = default);
    }
}
