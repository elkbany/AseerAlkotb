using AseerAlkotb.Domain.Entites.Models;
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

        public async Task<Payment> GetByTransactionIdAsync(string transactionId)
        {
            // Query الـ DB للبحث بالـ TransactionId
            return await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }
    }
}
