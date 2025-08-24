using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace AseerAlkotb.API.DependencyInjection
{
    public static class LocalizationServiceRegistration
    {
        public static IServiceCollection AddLocalizationServices(this IServiceCollection services)
        {
            // 1- Add Localization
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // 2- Add Controllers with Localization
            services.AddControllers()
                    .AddDataAnnotationsLocalization()
                    .AddViewLocalization();



            return services;
        }

        public static IApplicationBuilder UseLocalizationConfiguration(this IApplicationBuilder app)
        {
            // 3- Supported Cultures
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("ar")
            };

            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ar"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new AcceptLanguageHeaderRequestCultureProvider(), // يقرأ Accept-Language header
                    new QueryStringRequestCultureProvider(),          // يقرأ ?culture=ar
                    new CookieRequestCultureProvider()                // يقرأ من Cookie
                }
            };

            app.UseRequestLocalization(localizationOptions);
            return app;
        }
    }
}