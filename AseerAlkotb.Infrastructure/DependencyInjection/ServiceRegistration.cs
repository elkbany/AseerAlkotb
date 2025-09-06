using AseerAlkotb.Application.Contracts.External;
using AseerAlkotb.Infrastructure.ExternalServices;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Infrastructure.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            #region Cloudinary Service
            var cloudName = config["Cloudinary:CloudName"];
            var apiKey = config["Cloudinary:ApiKey"];
            var apiSecret = config["Cloudinary:ApiSecret"];

            if (!string.IsNullOrEmpty(cloudName) && !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(apiSecret))
            {
                var account = new Account(cloudName, apiKey, apiSecret);
                var cloudinary = new Cloudinary(account);

                services.AddSingleton(cloudinary);
                services.AddScoped<ICloudinaryService, CloudinaryService>();
            }
            else
            {
                // Log warning instead of throwing exception
                Console.WriteLine("Warning: Cloudinary configuration is missing or incomplete. Cloudinary service will not be available.");
            }

            #endregion

            return services;
        }

    }
}
