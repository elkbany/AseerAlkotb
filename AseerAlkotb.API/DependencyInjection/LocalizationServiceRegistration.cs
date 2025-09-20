﻿using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using System.Globalization;
using AseerAlkotb.Localization.Resources;

namespace AseerAlkotb.API.DependencyInjection
{
    public static class LocalizationServiceRegistration
    {
        public static IServiceCollection AddLocalizationServices(this IServiceCollection services)
        {
            // 1- Add Localization without specifying ResourcesPath since SharedResources is in a different project
            services.AddLocalization();

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
                DefaultRequestCulture = new RequestCulture("ar"), // Default to Arabic
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new AcceptLanguageHeaderRequestCultureProvider(), // يقرأ Accept-Language header (أولوية عالية)
                    new QueryStringRequestCultureProvider(),          // يقرأ ?culture=ar
                    new CookieRequestCultureProvider()                // يقرأ من Cookie
                },
                // Enable fallback to default culture if requested culture is not supported
                FallBackToParentCultures = true,
                FallBackToParentUICultures = true
            };

            app.UseRequestLocalization(localizationOptions);
            return app;
        }
    }
}