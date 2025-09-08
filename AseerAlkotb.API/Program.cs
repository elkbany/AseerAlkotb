using AseerAlkotb.API.DependencyInjection;
using AseerAlkotb.API.Extensions;
using AseerAlkotb.API.Middlewares;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Application.Contracts.External;
using AseerAlkotb.Application.Services;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Localization.Resources;

using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.ExternalServices;
using AseerAlkotb.Infrastructure.Repositories.Base;
using AseerAlkotb.Infrastructure.Repositories.Implementations;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;

using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using System.Reflection;
using AseerAlkotb.Infrastructure.Data;
using AseerAlkotb.Application.BackgroundJobs;
using AseerAlkotb.Infrastructure.Background;
using AseerAlkotb.Infrastructure.AI;
namespace AseerAlkotb.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Context Registeration
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString).UseLazyLoadingProxies();
            });
            #endregion

            #region Localization
            builder.Services.AddLocalizationServices();
            builder.Services.AddHttpContextAccessor();
            #endregion

        
            #region Identity Registration
            builder.Services.AddIdentity<User, IdentityRole<int>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                // Gnerates the default token providers for password reset, email confirmation, etc.
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // User Cannot Login without confirming email
                options.SignIn.RequireConfirmedEmail = true;
            });

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
            builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
            builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();


            builder.Services.AddScoped<ICartRepository, CartRepository>();

            builder.Services.AddScoped<IPublisherRepository , PublisherRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();

            #endregion

            #region Services
            builder.Services.AddScoped<IAuthorServices, AuthorServices>();
            builder.Services.AddScoped<IBookServices, BookServices>();
            builder.Services.AddScoped<ICategoryServices, CategoryServices>();
            builder.Services.AddScoped<ICartServices, CartServices>();
            builder.Services.AddScoped<IReviewServices, ReviewServices>();
            builder.Services.AddScoped<IPublisherServices, PublisherService>();
            builder.Services.AddScoped<IQuoteService, QuoteService>();
            builder.Services.AddScoped<IPaymobService, PaymobService>();
            builder.Services.AddScoped<IOrderServices, OrderServices>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<IAccountServices, AccountService>();
            builder.Services.AddScoped<IAdminServices, AdminServices>();
            
            // RAG Services
            builder.Services.AddScoped<IRagService, RagService>();
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

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.Configure<MvcOptions>(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            });

            #endregion

            #region HttpClient Registeration
            builder.Services.AddHttpClient<IPaymobService, PaymobService>();
            builder.Services.AddHttpClient();
            #endregion

            #region Cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost4200",
                    policy => policy.WithOrigins("http://localhost:4200")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod());
            });
            #endregion

            // HttpClient لِـ Gemini
            builder.Services.AddHttpClient("gemini", c =>
            {
                c.BaseAddress = new Uri("https://generativelanguage.googleapis.com");
            });

            // Embeddings + Synthesis على Gemini
            builder.Services.AddScoped<IEmbeddingService, GeminiEmbeddingService>();
            builder.Services.AddScoped<IAnswerSynthesisService, GeminiAnswerSynthesisService>();

            // لو هتوقفي الـ ExternalBookService (اختياري):
            // builder.Services.Remove(...)
            // أو ببساطة ما تستخدمهوش في Ask، وخلّيه للـ endpoint المخصص book-summary فقط.

            // الـ Background job (لو مش مسجل):
            builder.Services.AddSingleton<EmbeddingRefreshBackgroundService>();
            builder.Services.AddSingleton<IEmbeddingRefreshJob>(sp => sp.GetRequiredService<EmbeddingRefreshBackgroundService>());
            builder.Services.AddHostedService(sp => sp.GetRequiredService<EmbeddingRefreshBackgroundService>());


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

            #region seed roles
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await RoleSeeder.SeedRolesAsync(services);
            }
            #endregion
            // Use CORS
            app.UseCors("AllowLocalhost4200");

            #region Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "AseerAlktob API V1");
                });
            }
            #endregion

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            #region Localization
            app.UseLocalizationConfiguration();
            #endregion


            // Initialize LocalizerProvider correctly
            using (var scope = app.Services.CreateScope())
            {
                var localizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer<SharedResources>>();
                var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                LocalizerProvider.Init(localizer, httpContextAccessor);
            }

            app.UseHttpsRedirection();


            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            //app.Run();
            await app.RunAsync();
        }
    }
}
