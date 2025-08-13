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
using Microsoft.Extensions.FileProviders;
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

            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            builder.Services.AddScoped<ICartRepository, CartRepository>();

            builder.Services.AddScoped<IPublisherRepository , PublisherRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

            #endregion
            #region Services Registerations
            builder.Services.AddScoped<IAuthorServices,AuthorServices>();
            builder.Services.AddScoped<IBookServices, BookServices>();
            builder.Services.AddScoped<ICategoryServices, CategoryServices>();

            builder.Services.AddScoped<ICartServices, CartServices>();



            builder.Services.AddScoped<IReviewServices, ReviewServices>();


            builder.Services.AddScoped<IPublisherServices, PublisherService>();
            builder.Services.AddScoped<IWishlistServices, WishlistServices>();

            #endregion
            #region AutoMapper 
            builder.Services.AddMapster();
            TypeAdapterConfig.GlobalSettings.Compile();
            #endregion
            #region Validation
            builder.Services.AddFluentValidationValidators();
            #endregion


            #region Cors
            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost4200",
                    policy => policy.WithOrigins("http://localhost:4200")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod());
            });
            #endregion

            builder.Services.AddControllers();

            #region CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            #endregion

            #region Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            #endregion

            var app = builder.Build();
            #region Access Images
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")),
                RequestPath = "/uploads"
            });
            #endregion
            // Use CORS
            app.UseCors("AllowLocalhost4200");

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
