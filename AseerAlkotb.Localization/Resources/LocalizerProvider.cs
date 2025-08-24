using AseerAlkotb.Domain.Resources;
using FluentValidation;
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

        public static IStringLocalizer<SharedResources> Localizer
        {
            get
            {
                return _localizer;
            }
        }

        public static void Init(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }
    }

    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> L<T, TProperty>(
             this IRuleBuilderOptions<T, TProperty> ruleBuilder,
             params string[] keys)
        {
            var message = string.Join(" ", keys.Select(k =>
                LocalizerProvider.Localizer[k] ?? "not"
            ));
            return ruleBuilder.WithMessage(message);
        }

    }
}
