using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.City.DTOs;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using Microsoft.Extensions.Hosting;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class CityServices : AppService, ICityServices
    {
        private readonly IUnitOfWork unitOfWork;

        public CityServices(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IHostEnvironment environment) 
            : base(serviceProvider, environment)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<CityDto>>> GetCitiesByGovernorateAsync(int governorateId)
        {
            var governorate = await unitOfWork.Governorates.FirstOrDefaultAsync(g => g.Id == governorateId);
            if (governorate == null)
            {
                return NotFound<List<CityDto>>("Governorate not found");
            }

            var cities =  unitOfWork.Cities.GetQueryable(c=>c.GovernorateId==governorateId).Select
                (ci=> new CityDto
                {
                    Name = ci.Name,
                    Id = ci.Id
                }
                ).ToList();
            return Success(cities);
        }

        public async Task<ApiResponse<City>> AddCityAsync(string name, int governorateId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest<City>("City name is required");
            }

            var governorate = await unitOfWork.Governorates.FirstOrDefaultAsync(g => g.Id == governorateId);
            if (governorate == null)
            {
                return NotFound<City>("Governorate not found");
            }

            var existingCity = await unitOfWork.Cities.FirstOrDefaultAsync(c => c.Name == name && c.GovernorateId == governorateId);
            if (existingCity != null)
            {
                return BadRequest<City>("City with this name already exists in this governorate");
            }

            var city = new City 
            { 
                Name = name, 
                GovernorateId = governorateId 
            };
            
            await unitOfWork.Cities.InsertAsync(city);
            await unitOfWork.CommitAsync();

            return Success(city);
        }
    }
}