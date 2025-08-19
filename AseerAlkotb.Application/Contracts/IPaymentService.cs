using AseerAlkotb.Application.Features.Payments.Requests;
using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Domain.Entites;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Contracts
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(int userId, int orderId, decimal amount, PaymentMethod method);
        Task<Payment?> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<Payment>> GetPaymentsByUserAsync(int userId);
        Task<bool> UpdatePaymentStatusAsync(int paymentId, PaymentStatus status);
    }

}

