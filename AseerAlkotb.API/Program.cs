
using AseerAlkotb.API.Extensions;
using AseerAlkotb.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Mapster;
using MapsterMapper;
using System.Reflection;
using AseerAlkotb.API.Middlewares;
using AseerAlkotb.Infrastructure.Repositories.Implementations;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Services;
using AseerAlkotb.Domain.Interfaces.Base;
using AseerAlkotb.Infrastructure.Repositories.Base;
namespace AseerAlkotb.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region  Context Registeration
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString).UseLazyLoadingProxies();
            });
            #endregion
            #region Repositories Registerations
            builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            #endregion
            #region Services Registerations
            builder.Services.AddScoped<IAuthorServices,AuthorServices>();
            builder.Services.AddScoped<ICategoryServices, CategoryServices>();

            #endregion
            #region AutoMapper 
            builder.Services.AddMapster();
            #endregion
            #region Validation
            //builder.Services.AddFluentValidationValidators();
            #endregion


            builder.Services.AddControllers();
            #region Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                #region Swagger
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "AseerAlktob API V1");
                    //// Add these for better CORS handling
                    //options.EnableTryItOutByDefault();
                    //options.DisplayRequestDuration();
                });
                #endregion
            }
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
