using AseerAlkotb.Application.Features.Governorates.DTOs;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;

namespace AseerAlkotb.Application.Contracts
{
    public interface IGovernorateServices
    {
        Task<ApiResponse<List<GovernorateDto>>> GetAllGovernoratesAsync();
        Task<ApiResponse<Governorate>> AddGovernorateAsync(string name);
    }
}