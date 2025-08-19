using AseerAlkotb.API.Extensions;
using AseerAlkotb.API.Middlewares;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Services;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;
using AseerAlkotb.Infrastructure.Repositories.Implementations;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using AseerAlkotb.Domain.Entites.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using AseerAlkotb.Application.Features.Account.Validator;
using FluentValidation;
using AseerAlkotb.Application.Features.Account.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using System.Reflection;
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
            #region Identity Registration
            builder.Services.AddIdentity<User, IdentityRole<int>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            //.AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultScheme= JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.SaveToken=true;
                option.RequireHttpsMetadata = true;//http=false
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:IssuerIP"],
                    ValidateAudience = true,
                    ValidAudience= builder.Configuration["JWT:AudienceIP"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
                };
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

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            #endregion
            #region Services Registerations
            builder.Services.AddScoped<IAuthorServices,AuthorServices>();
            builder.Services.AddScoped<IBookServices, BookServices>();
            builder.Services.AddScoped<ICategoryServices, CategoryServices>();
            builder.Services.AddScoped<ICartServices, CartServices>();
            builder.Services.AddScoped<IReviewServices, ReviewServices>();
            builder.Services.AddScoped<IPublisherServices, PublisherService>();
            builder.Services.AddScoped<IPaymobService, PaymobService>();
            builder.Services.AddScoped<IOrderServices, OrderServices>();
            builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

            #endregion
            #region AutoMapper 
            builder.Services.AddMapster();
            TypeAdapterConfig.GlobalSettings.Compile();
            #endregion
            #region Validation
            builder.Services.AddFluentValidationValidators();

            #endregion

            #region HttpClient Registeration
            builder.Services.AddHttpClient<IPaymobService, PaymobService>();
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

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
