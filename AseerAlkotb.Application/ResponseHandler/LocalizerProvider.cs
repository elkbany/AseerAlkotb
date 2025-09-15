using AseerAlkotb.Localization.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.ResponseHandler
{
    public static class LocalizerProvider
    {
        private static IStringLocalizer<SharedResources> _localizer;
        private static IHttpContextAccessor _httpContextAccessor;
        private static IServiceProvider _serviceProvider;
        private static IStringLocalizerFactory _localizerFactory;

        public static IStringLocalizer<SharedResources> Localizer
        {
            get
            {
                if (_localizer == null && _localizerFactory != null)
                {
                    _localizer = (IStringLocalizer<SharedResources>)_localizerFactory.Create(typeof(SharedResources));
                }
                return _localizer;
            }
        }

        public static void Init(IStringLocalizer<SharedResources> localizer, IHttpContextAccessor httpContextAccessor = null, IServiceProvider serviceProvider = null)
        {
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
            
            if (serviceProvider != null)
            {
                _localizerFactory = serviceProvider.GetRequiredService<IStringLocalizerFactory>();
            }
        }

        public static void RefreshLocalizer()
        {
            try
            {
                if (_localizerFactory != null)
                {
                    // إنشاء localizer جديد من الـ factory
                    _localizer = (IStringLocalizer<SharedResources>)_localizerFactory.Create(typeof(SharedResources));
                    Console.WriteLine("Localizer refreshed successfully");
                }
                else if (_serviceProvider != null)
                {
                    // fallback: استخدام الـ service provider
                    using var scope = _serviceProvider.CreateScope();
                    _localizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer<SharedResources>>();
                    Console.WriteLine("Localizer refreshed successfully via service provider");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to refresh localizer: {ex.Message}");
            }
        }

        // Helper method to get localized message
        public static string GetLocalizedMessage(string key, params object[] args)
        {
            if (_localizer == null)
                return key;

            var localizedString = _localizer[key];
            var message = localizedString.Value; // Access the Value property of LocalizedString
            if (args.Length > 0)
            {
                message = string.Format(message, args);
            }
            return message;
        }

        // Method to get localized message with culture awareness
        public static string GetLocalizedMessageWithCulture(string key, params object[] args)
        {
            if (_localizer == null)
                return key;

            // Try to get the current culture from HttpContext if available
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            if (_httpContextAccessor?.HttpContext != null)
            {
                var requestCulture = _httpContextAccessor.HttpContext.Features.Get<Microsoft.AspNetCore.Localization.IRequestCultureFeature>();
                if (requestCulture?.RequestCulture?.Culture != null)
                {
                    culture = requestCulture.RequestCulture.Culture;
                }
            }

            var localizedString = _localizer[key];
            var message = localizedString.Value; // Access the Value property of LocalizedString
            if (args.Length > 0)
            {
                message = string.Format(message, args);
            }
            return message;
        }
    }

    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> L<T, TProperty>(
             this IRuleBuilderOptions<T, TProperty> ruleBuilder,
             params string[] keys)
        {
            var message = string.Join(" ", keys.Select(k =>
                LocalizerProvider.GetLocalizedMessageWithCulture(k) ?? k
            ));
            return ruleBuilder.WithMessage(message);
        }

        // Overload for messages with parameters
        public static IRuleBuilderOptions<T, TProperty> L<T, TProperty>(
             this IRuleBuilderOptions<T, TProperty> ruleBuilder,
             string key,
             params object[] args)
        {
            var message = LocalizerProvider.GetLocalizedMessageWithCulture(key, args) ?? key;
            return ruleBuilder.WithMessage(message);
        }
    }
}
