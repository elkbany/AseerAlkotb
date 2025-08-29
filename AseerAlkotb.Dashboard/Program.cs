using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Contracts.External;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Application.Services;
using AseerAlkotb.Domain.Interfaces.Base;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.ExternalServices;
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

            // Add DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });

            // Add Repositories
            builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add Services
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

            #region HttpClient Registeration
            builder.Services.AddHttpClient<IPaymobService, PaymobService>();
            #endregion

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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}