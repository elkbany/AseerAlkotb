using AseerAlkotb.Application.Features.Account.Requests;
using AseerAlkotb.Application.Features.Account.Responses;
using AseerAlkotb.Application.ResponseHandler;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Contracts
{
    public interface IAccountServices
    {
        Task<ApiResponse<RegisterResponse>> CreateAccount(RegisterRequest request);
        Task<ApiResponse<RegisterResponse>> Register(RegisterRequest request);

        Task<ApiResponse<string>> ConfirmEmail(string userId, string token);

        Task<ApiResponse<LoginResponse>> Login(LoginRequest request);
        Task<ApiResponse<string>> ResendEmailConfirmation(string email);
        Task<ApiResponse<string>> ForgotPassword(string email);
        Task<ApiResponse<string>> ResetPassword(ResetPasswordRequest request);
    }
}
