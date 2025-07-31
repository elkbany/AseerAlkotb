using FluentValidation;
using System.Reflection;

namespace AseerAlkotb.API.Extensions
{
    public static class FluentValidationServiceCollections
    {
            public static void AddFluentValidationValidators(this IServiceCollection services)
            {
                var validatorTypes = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => !t.IsAbstract && t.IsAssignableToGenericType(typeof(AbstractValidator<>)))
                    .ToList();

                foreach (var validatorType in validatorTypes)
                {
                    var genericArg = validatorType.BaseType!.GetGenericArguments().FirstOrDefault();
                    if (genericArg != null)
                    {
                        var serviceType = typeof(IValidator<>).MakeGenericType(genericArg);
                        services.AddScoped(serviceType, validatorType);
                    }
                }
            }

            // Extension method to check if a type is assignable to a generic type
            private static bool IsAssignableToGenericType(this Type givenType, Type genericType)
            {
                var interfaceTypes = givenType.GetInterfaces();

                foreach (var it in interfaceTypes)
                {
                    if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                        return true;
                }

                if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                    return true;

                Type? baseType = givenType.BaseType;
                if (baseType == null) return false;

                return IsAssignableToGenericType(baseType, genericType);
            }
        }
    }

