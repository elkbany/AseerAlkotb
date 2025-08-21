using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Account.Requests;
using AseerAlkotb.Application.Features.Account.Responses;
using AseerAlkotb.Application.ResponseHandler;

namespace AseerAlkotb.Application.Contracts
{
    public interface IAccountServices
    {
        Task<ApiResponse<RegisterResponse>> CreateAccount(RegisterRequest request);
        Task<ApiResponse<LoginResponse>> Login(LoginRequest request);
    }
}
