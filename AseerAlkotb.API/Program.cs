using AseerAlkotb.API.DependencyInjection;
using AseerAlkotb.API.Extensions;
using AseerAlkotb.API.Middlewares;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Contracts.External;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Application.Services;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Data;
using AseerAlkotb.Infrastructure.DependencyInjection;
using AseerAlkotb.Infrastructure.ExternalServices;
using AseerAlkotb.Infrastructure.Repositories.Base;
using AseerAlkotb.Infrastructure.Repositories.Implementations;
using AseerAlkotb.Localization.Resources;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
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
            builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                // Configure password requirements if needed
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
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
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.SaveToken = true;
                option.RequireHttpsMetadata = false; // Set to false for development (localhost)
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:IssuerIP"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:AudienceIP"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
                    ClockSkew = TimeSpan.Zero // Removes the default 5-minute clock skew
                };

                // Handle JWT Bearer events for better debugging
                option.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validated successfully");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"JWT Challenge: {context.Error}, {context.ErrorDescription}");
                        return Task.CompletedTask;
                    }
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
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            #endregion

            #region Services
            builder.Services.AddScoped<IAuthorServices, AuthorServices>();
            builder.Services.AddScoped<IBookServices, BookServices>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<ICartServices, CartServices>();
            builder.Services.AddScoped<IReviewServices, ReviewServices>();
            builder.Services.AddScoped<IPublisherServices, PublisherService>();
            builder.Services.AddScoped<IQuoteService, QuoteService>();
            builder.Services.AddScoped<IPaymobService, PaymobService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IOrderServices, OrderServices>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddSingleton<IChatLogStore, API.DependencyInjection.InMemoryChatLogStore>();

            builder.Services.AddScoped<IAccountServices, AccountService>();
            builder.Services.AddScoped<IAdminServices, AdminServices>();


            builder.Services.AddInfrastructure(builder.Configuration);


            builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
            builder.Services.AddScoped<IWishlistServices, WishlistServices>();
            builder.Services.AddScoped<ICategoryServices, CategoryServices>();
            
            // New services for improved Order and Payment flow
            builder.Services.AddScoped<IOrderPaymentSyncService, OrderPaymentSyncService>();
            builder.Services.AddScoped<IPaymentRetryService, PaymentRetryService>();

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

            #region Rate Limiting
            builder.Services.AddRateLimiter(options =>
            {
                // Callback endpoint rate limiting - 10 requests per minute per IP
                options.AddFixedWindowLimiter("CallbackPolicy", opt =>
                {
                    opt.PermitLimit = 10;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0; // No queuing, reject immediately when limit exceeded
                });

                // Webhook endpoint rate limiting - 20 requests per minute per IP
                options.AddFixedWindowLimiter("WebhookPolicy", opt =>
                {
                    opt.PermitLimit = 20;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                // Global rate limiting fallback
                options.GlobalLimiter = System.Threading.RateLimiting.PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                        factory: partition => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1)
                        }));

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
            #endregion

            #region Cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost4200",
                    policy => policy.WithOrigins("http://localhost:4200")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials()); // Added for authentication support
            });
            #endregion

            #region Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new() { Title = "AseerAlktob", Version = "v1" });

                // Add JWT Bearer definition
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: Bearer abc123xyz"
                });

                // Make sure Swagger requires this scheme
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
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

            // IMPORTANT: CORS must be before Authentication and Authorization
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

            // Add rate limiting middleware
            app.UseRateLimiter();

            // IMPORTANT: Order matters - Authentication before Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}