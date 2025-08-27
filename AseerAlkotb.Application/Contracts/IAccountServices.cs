using AseerAlkotb.Application.Contracts.External;
using AseerAlkotb.Application.Features.Account.Requests;
using AseerAlkotb.Application.Features.Account.Responses;
using AseerAlkotb.Application.Features.Account.Validator;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Contracts
{
    public interface IAccountServices
    {
        Task<ApiResponse<RegisterResponse>> Register(RegisterRequest request);
        Task<ApiResponse<string>> ConfirmEmail(string userId, string token);
        Task<ApiResponse<string>> ForgotPassword(string email);
        Task<ApiResponse<string>> ResetPassword(ResetPasswordRequest request);
        Task<ApiResponse<LoginResponse>> Login(LoginRequest request);
        Task<ApiResponse<string>> ResendEmailConfirmation(string email);
        Task<ApiResponse<UpdateProfileResponse>> UpdateProfile(int userId, UpdateProfileRequest request);     
        Task<ApiResponse<GetProfileResponse>> GetProfile(GetProfileRequest request);
    }
}
