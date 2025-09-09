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
                var connectionString = builder.Configuration.GetConnectionString("Shared");
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

            // Add other services needed by the dashboard controllers


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
                LocalizerProvider.Init(localizer, httpContextAccessor);
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