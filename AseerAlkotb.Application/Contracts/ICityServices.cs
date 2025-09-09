using AseerAlkotb.Application.Features.City.DTOs;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;

namespace AseerAlkotb.Application.Contracts
{
    public interface ICityServices
    {
        Task<ApiResponse<List<CityDto>>> GetCitiesByGovernorateAsync(int governorateId);
        Task<ApiResponse<City>> AddCityAsync(string name, int governorateId);
    }
}