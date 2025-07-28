using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AseerAlkotb.Application.Services
{
    public abstract class AppService
    {
        private readonly IServiceProvider serviceProvider;

        protected AppService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        protected async Task DoValidationAsync<TValidator, TRequest>(TRequest request, params object[] constructorParameters)
          where TValidator : AbstractValidator<TRequest>
        {
            var validator = serviceProvider.GetRequiredService<TValidator>();
            var result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            //var instance = (TValidator)Activator.CreateInstance(typeof(TValidator), constructorParameters)!;

            //var validateResult = await instance.ValidateAsync(request);
            //if (!validateResult.IsValid)
            //{
            //    throw new ValidationException(validateResult.Errors);
            //}
        }
    }
}
