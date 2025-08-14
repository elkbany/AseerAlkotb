using AseerAlkotb.Application.Features.Payments.Requests;
using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Domain.Entites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Contracts
{
    public interface IPaymentService
    {

    // بدء الدفع: يرجع Response مع URL إذا لزم
    Task<InitiatePaymentResponse> InitiatePaymentAsync(InitiatePaymentRequest request);

        // معالجة الـ Callback من البوابة: تحدث الـ Status
        Task ProcessCallbackAsync(PaymentCallbackRequest request); 
    }
}

