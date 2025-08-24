using AseerAlkotb.Domain.Entites;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Infrastructure.Repositories.Implementations
{
    public class PaymentRepository : GenericRepository<Payment, int>, IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Payment?> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                                 .Include(p => p.User)
                                 .Include(p => p.Order)
                                 .FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);
        }

        public async Task<List<Payment>> GetPaymentsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                                 .Include(p => p.Order)
                                 .Where(p => p.UserId == userId)
                                 .ToListAsync(cancellationToken);
        }

        public async Task UpdatePaymentStatusAsync(int paymentId, string status, CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments.FindAsync(new object[] { paymentId }, cancellationToken);
            if (payment != null)
            {
                payment.Status = status;
                _context.Payments.Update(payment);
            }
        }


        public async Task<List<Payment>> GetPaymentsAsync(
            int? userId = null,
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Payments
                                .Include(p => p.User)
                                .Include(p => p.Order)
                                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(p => p.UserId == userId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(p => p.Status == status );

            if (fromDate.HasValue)
                query = query.Where(p => p.PaymentDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(p => p.PaymentDate <= toDate.Value);

            return await query.ToListAsync(cancellationToken);
        }
    }
}
