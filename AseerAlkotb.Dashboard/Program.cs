using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Contracts.External;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Application.Services;
using AseerAlkotb.Infrastructure.ExternalServices;

using Mapster;
using AseerAlkotb.Infrastructure.Repositories.Base;
using AseerAlkotb.Localization.Resources;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.ExternalServices;
using AseerAlkotb.Infrastructure.Repositories.Base;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using AseerAlkotb.Infrastructure.DependencyInjection;
using AseerAlkotb.Infrastructure.AI;
using AseerAlkotb.Infrastructure.Background;
using AseerAlkotb.Application.BackgroundJobs;
using Polly.Extensions.Http;
using Polly;
using System.Net;
using AseerAlkotb.Application.Features.Account.Validator;
using AseerAlkotb.Application.Features.Authors.Validators;
using AseerAlkotb.Application.Features.Books.Validators;
using AseerAlkotb.Application.Features.CartItems.Validation;
using AseerAlkotb.Application.Features.Categories.Validators;
using AseerAlkotb.Application.Features.Orders.Validators;
using AseerAlkotb.Application.Features.Payments.Validators;
using AseerAlkotb.Application.Features.Publishers.Validators;
using AseerAlkotb.Application.Features.Quotes.Validators;
using AseerAlkotb.Application.Features.Rag.Validators;
using AseerAlkotb.Application.Features.Reviews.Validators;
using AseerAlkotb.Application.Features.Roles.Validators;
using AseerAlkotb.Application.Features.Wishlist.Validators;

namespace AseerAlkotb.Dashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            #region Context and Identity
            // Add DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });


            // Add Identity
            builder.Services.AddIdentity<User, IdentityRole<int>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                // Gnerates the default token providers for password reset, email confirmation, etc.
                .AddDefaultTokenProviders();


            // Add Identity services to the container.
            //builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
            //{
            //    // Configure Identity options if needed
            //    options.SignIn.RequireConfirmedEmail = true;
            //})
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddDefaultTokenProviders();


            #endregion

            builder.Services.AddHttpContextAccessor();


            #region Repositories and Services

            // Add Repositories
            builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            #endregion
            // Add Application Services
            builder.Services.AddScoped<IAuthorServices, AuthorServices>();
            builder.Services.AddScoped<IBookServices, BookServices>();
            builder.Services.AddScoped<ICategoryServices, CategoryServices>();
            builder.Services.AddScoped<IGovernorateServices, GovernorateServices>();
            builder.Services.AddScoped<ICityServices, CityServices>();
            builder.Services.AddScoped<IOrderServices, OrderServices>();
            builder.Services.AddScoped<IAdminServices, AdminServices>();

            #region Infrastructure Services
            builder.Services.AddInfrastructure(builder.Configuration);
            #endregion


            builder.Services.AddScoped<IPublisherServices, PublisherService>();
            builder.Services.AddScoped<IReviewServices, ReviewServices>();
            builder.Services.AddScoped<IPublisherServices, PublisherService>();
            builder.Services.AddScoped<IQuoteService, QuoteService>();
            builder.Services.AddScoped<IPaymobService, PaymobService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IOrderServices, OrderServices>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            #region HttpClient Registeration
            builder.Services.AddHttpClient<IPaymobService, PaymobService>();
            #endregion
            builder.Services.AddScoped<IAccountServices, AccountService>();
            // New services for improved Order and Payment flow
            builder.Services.AddScoped<IOrderPaymentSyncService, OrderPaymentSyncService>();
            builder.Services.AddScoped<IPaymentRetryService, PaymentRetryService>();
            #region Validators 
            // Account Validators
            builder.Services.AddScoped<GetProfileRequestValidator>();
            builder.Services.AddScoped<LoginRequestValidator>();
            builder.Services.AddScoped<RegisterRequestValidator>();
            builder.Services.AddScoped<ResetPasswordRequestValidator>();
            builder.Services.AddScoped<UpdateProfileRequestValidator>();

            // Authors Validators
            builder.Services.AddScoped<AddAuthorRequestValidator>();
            builder.Services.AddScoped<DeleteAuthorRequestValidator>();
            builder.Services.AddScoped<FollowAuthorRequestValidation>();
            builder.Services.AddScoped<GetAllAuthorsPaginatedRequestValidator>();
            builder.Services.AddScoped<GetAutherFollowerCountRequestValidation>();
            builder.Services.AddScoped<GetFollowedAuthorRequestValidatin>();
            builder.Services.AddScoped<GetFollowersAuthorRequestValidation>();
            builder.Services.AddScoped<UnFollowAuthorRequestValidation>();
            builder.Services.AddScoped<UpdateAuthorRequestValidator>();
            builder.Services.AddScoped<GetAuthorByIdRequestValidator>();


            // Books Validators
            builder.Services.AddScoped<AddBookRequestValidator>();
            builder.Services.AddScoped<DeleteBookRequestValidator>();
            builder.Services.AddScoped<FilterBooksRequestValidator>();
            builder.Services.AddScoped<GetAllBooksPaginatedValidator>();
            builder.Services.AddScoped<GetBookByIdRequestValidator>();
            builder.Services.AddScoped<UpdateBookRequestValidator>();

            // Cart Items Validators
            builder.Services.AddScoped<AddCartItemValidation>();
            builder.Services.AddScoped<ClearCartRequestValidation>();
            builder.Services.AddScoped<DeleteItemValidation>();
            builder.Services.AddScoped<ShowCartRequestValidation>();
            builder.Services.AddScoped<UpdateItemQuantityValidation>();

            // Category Validators
            builder.Services.AddScoped<AddCategoryRequestValidator>();
            builder.Services.AddScoped<AddSubCategoryRequestValidator>();
            builder.Services.AddScoped<DeleteCategoryRequestValidator>();
            builder.Services.AddScoped<DeleteSubCategoryRequestValidator>();
            builder.Services.AddScoped<GetAllCategoriesPaginatedRequestValidator>();
            builder.Services.AddScoped<GetAllSubCategoriesPaginatedRequestValidator>();
            builder.Services.AddScoped<GetCategoryByIdRequestValidator>();
            builder.Services.AddScoped<UpdateCategoryRequestValidator>();

            // Order Validators
            builder.Services.AddScoped<AddOrderRequestValidator>();
            builder.Services.AddScoped<CancelOrderRequestValidator>();
            builder.Services.AddScoped<GetOrderByAdminByTrackingNumberRequestValidator>();
            builder.Services.AddScoped<GetAllUserOrdersPaginatedRequestValidator>();
            builder.Services.AddScoped<GetOrderByAdminByTrackingNumberRequestValidator>();
            builder.Services.AddScoped<GetUserOrderByTrackingNumberRequestValidator>();
            builder.Services.AddScoped<OrderItemDTOValidator>();
            builder.Services.AddScoped<UpdateOrderStatusRequestValidator>();
            builder.Services.AddScoped<GetAllOrdersPaginatedRequestValidator>();

            // Payment Validators
            builder.Services.AddScoped<InitializePaymentRequestValidator>();
            builder.Services.AddScoped<UpdatePaymentStatusRequestValidator>();

            // Publisher Validators
            builder.Services.AddScoped<AddPublisherRequestValidator>();
            builder.Services.AddScoped<DeletePublisherRequestValidator>();
            builder.Services.AddScoped<FollowPublisherRequestValidation>();
            builder.Services.AddScoped<GetAllPublishersPaginatedRequestValidator>();
            builder.Services.AddScoped<GetFollowedPublisherRequestValidation>();
            builder.Services.AddScoped<GetFollowersPublisherRequestValidation>();
            builder.Services.AddScoped<GetPublisherByIdRequestValidator>();
            builder.Services.AddScoped<GetPublisherFollowerCountRequestValidation>();
            builder.Services.AddScoped<UnFollowPublisherRequestValidation>();
            builder.Services.AddScoped<UpdatePublisherRequestValidator>();

            // Quote Validators
            builder.Services.AddScoped<AddQuoteRequestValidator>();
            builder.Services.AddScoped<DeleteQuoteRequestValidator>();
            builder.Services.AddScoped<GetAllQuotePaginatedRequestValidator>();
            builder.Services.AddScoped<GetByIdQuoteRequestValidator>();
            builder.Services.AddScoped<UpdateQuoteRequestValidator>();

            // Rag Validators
            builder.Services.AddScoped<RagAskRequestValidator>();

            // Review Validators
            builder.Services.AddScoped<AddReviewRequestValidator>();
            builder.Services.AddScoped<DeleteReviewRequestValidator>();
            builder.Services.AddScoped<GetAllReviewsPaginatedRequestValidator>();
            builder.Services.AddScoped<GetReviewByIdRequestValidator>();
            builder.Services.AddScoped<UpdateReviewRequestValidator>();

            // Role Validators
            builder.Services.AddScoped<AssignRoleRequestValidator>();
            builder.Services.AddScoped<CreateAdminAccountRequestValidator>();
            builder.Services.AddScoped<DeleteAdminAccountRequestValidator>();
            builder.Services.AddScoped<RemoveRoleRequestValidator>();
            builder.Services.AddScoped<UpdateAdminAccountRequestValidator>();

            // Wishlist Validators
            builder.Services.AddScoped<AddWishlistItemValidation>();
            builder.Services.AddScoped<ClearWishlistValidation>();
            builder.Services.AddScoped<DeleteWishlistItemValidation>();
            builder.Services.AddScoped<GetUserWishlistValidation>();
            builder.Services.AddScoped<GetWishlistItemCountValidation>();
            builder.Services.AddScoped<IsBookInWishlistValidation>();
            #endregion
            // Add other services needed by the dashboard controllers

            // RAG deps (Embedding + Router)
            builder.Services.AddScoped<IEmbeddingService, GeminiEmbeddingService>();
            //builder.Services.AddScoped<IQuestionRouterService, GeminiQuestionRouterService>();

            // Background job for embeddings (Dashboard فقط)
            builder.Services.AddSingleton<EmbeddingRefreshBackgroundService>();
            builder.Services.AddSingleton<IEmbeddingRefreshJob>(sp => sp.GetRequiredService<EmbeddingRefreshBackgroundService>());
            builder.Services.AddHostedService(sp => sp.GetRequiredService<EmbeddingRefreshBackgroundService>());
            // HttpClient (Gemini) مع Polly
            // Polly (اختياري)
            static IAsyncPolicy<HttpResponseMessage> ResilientPolicy() =>
                HttpPolicyExtensions.HandleTransientHttpError()
                    .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                    .WaitAndRetryAsync(3, a => TimeSpan.FromMilliseconds(400 * a * a));

            // HttpClient باسمي "gemini"
            builder.Services.AddHttpClient("gemini", c =>
            {
                c.BaseAddress = new Uri("https://generativelanguage.googleapis.com");
                // c.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddPolicyHandler(ResilientPolicy());


            // Configure Mapster for object mapping
            #region Localization
            builder.Services.AddLocalization();
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "ar" };
                options.SetDefaultCulture(supportedCultures[0])
                       .AddSupportedCultures(supportedCultures)
                       .AddSupportedUICultures(supportedCultures);
            });
            builder.Services.AddHttpContextAccessor();
            #endregion
            // Configure Mapster
            builder.Services.AddMapster();
            TypeAdapterConfig.GlobalSettings.Compile();

            // Register services for external dependencies, only if needed by the dashboard
            // For example, if you need email sending in your dashboard controllers
            builder.Services.AddScoped<IEmailService, EmailService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            #region Localization
            app.UseRequestLocalization();
            #endregion

            // Initialize LocalizerProvider correctly
            using (var scope = app.Services.CreateScope())
            {
                var localizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer<SharedResources>>();
                var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                LocalizerProvider.Init(localizer, httpContextAccessor, app.Services);
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles(); // Serves static files from wwwroot

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "account",
                pattern: "{controller=Account}/{action=Login}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}