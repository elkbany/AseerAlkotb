using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;
using AseerAlkotb.Domain.Interfaces.Base;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Contracts.External;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Application.Services;
using AseerAlkotb.Application.Contracts.External;
using AseerAlkotb.Infrastructure.ExternalServices;
using Mapster;
using AseerAlkotb.Infrastructure.Repositories.Base;
using AseerAlkotb.Localization.Resources;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

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

            // Add Identity services to the container.
            builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                // Configure Identity options if needed
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            #endregion

            #region Repositories and Services
            // Add Repositories
            builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add Application Services
            builder.Services.AddScoped<IAuthorServices, AuthorServices>();
            builder.Services.AddScoped<IBookServices, BookServices>();
            builder.Services.AddScoped<ICategoryServices, CategoryServices>();
            builder.Services.AddScoped<IOrderServices, OrderServices>();
            builder.Services.AddScoped<IPublisherServices, PublisherService>();
            builder.Services.AddScoped<IReviewServices, ReviewServices>();
            builder.Services.AddScoped<IAccountServices, AccountService>();
            builder.Services.AddScoped<IAdminServices, AdminServices>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            // Add other services needed by the dashboard controllers
            #endregion

            #region Other Configurations
            // Configure Mapster for object mapping
            #region Localization
            builder.Services.AddLocalization();
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "en", "ar" };
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
            #endregion

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
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}