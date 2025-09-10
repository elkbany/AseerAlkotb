using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Governorates.DTOs;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class GovernorateServices : AppService, IGovernorateServices
    {
        private readonly IUnitOfWork unitOfWork;

        public GovernorateServices(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IHostEnvironment environment) 
            : base(serviceProvider, environment)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<GovernorateDto>>> GetAllGovernoratesAsync()
        {
            var governorates = await unitOfWork.Governorates.GetQueryable().Select
                (g => new GovernorateDto
                {
                    Id = g.Id,
                    Nmae = g.Name
                }
                ).ToListAsync();
            return Success(governorates);
        }

        public async Task<ApiResponse<Governorate>> AddGovernorateAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest<Governorate>("Governorate name is required");
            }

            var existingGovernorate = await unitOfWork.Governorates.FirstOrDefaultAsync(g => g.Name == name);
            if (existingGovernorate != null)
            {
                return BadRequest<Governorate>("Governorate with this name already exists");
            }

            var governorate = new Governorate { Name = name };
            await unitOfWork.Governorates.InsertAsync(governorate);
            await unitOfWork.CommitAsync();

            return Success(governorate);
        }
    }
}