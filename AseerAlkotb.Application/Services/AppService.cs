using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Localization.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Hosting;  // For IWebHostEnvironment
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using AseerAlkotb.Application.ResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public abstract class AppService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IHostEnvironment _environment;

        private IStringLocalizer<SharedResources>? _localizer;

        protected IStringLocalizer<SharedResources> _stringLocalizer =>
            _localizer ??= serviceProvider.GetRequiredService<IStringLocalizer<SharedResources>>();

        protected AppService(IServiceProvider serviceProvider, IHostEnvironment environment)
        {
            this.serviceProvider = serviceProvider;
            _environment = environment;
        }

        #region Validate Async
        protected async Task DoValidationAsync<TValidator, TRequest>(TRequest request, params object[] constructorParameters)
        where TValidator : AbstractValidator<TRequest>
        {
            var instance = (TValidator)Activator.CreateInstance(typeof(TValidator), constructorParameters)!;

            var validateResult = await instance.ValidateAsync(request);
            if (!validateResult.IsValid)
            {
                throw new ValidationException(validateResult.Errors);
            }
        }
        #endregion

        #region Uploading Files
        public async Task<string> UploadImageAsync(IFormFile imageFile, string folder)
        {
            var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedContentTypes.Contains(imageFile.ContentType.ToLowerInvariant()))
                throw new ArgumentException("Invalid image file");

            // Generate unique filename
            var fileName = GenerateUniqueFileName(imageFile.FileName);

            // Get wwwroot path using current directory
            var currentDirectory = Directory.GetCurrentDirectory();
            var wwwrootPath = Path.Combine(currentDirectory, "wwwroot");
            var uploadsFolder = Path.Combine(wwwrootPath, "uploads", folder);

            // Create directory if it doesn't exist
            Directory.CreateDirectory(uploadsFolder);

            // Full file path
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Return URL path
            return $"/uploads/{folder}/{fileName}";
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return false;

            try
            {
                // Get wwwroot path
                var currentDirectory = Directory.GetCurrentDirectory();
                var wwwrootPath = Path.Combine(currentDirectory, "wwwroot");

                // Convert URL to physical path
                var relativePath = imageUrl.TrimStart('/');
                var filePath = Path.Combine(wwwrootPath, relativePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
            }
            catch (Exception)
            {
                // Log exception
                return false;
            }
            return false;
        }

        public async Task<string> UpdateImageAsync(IFormFile newImage, string oldImageUrl, string folder)
        {
            // Upload new image
            var newImageUrl = await UploadImageAsync(newImage, folder);

            // Delete old image
            if (!string.IsNullOrEmpty(oldImageUrl))
            {
                await DeleteImageAsync(oldImageUrl);
            }

            return newImageUrl;
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var uniqueName = $"{Guid.NewGuid()}{extension}";
            return uniqueName;
        }
        #endregion

        #region Localization Helpers
        protected string LocalizeOr(string key, string fallback)
        {
            var localized = _stringLocalizer[key];
            if (localized.ResourceNotFound || string.IsNullOrWhiteSpace(localized.Value))
            {
                return fallback;
            }
            return localized.Value;
        }

        protected string LocalizeEntity(string entityType, int id, string field, string fallback)
        {
            var key = $"{entityType}_{id}_{field}";
            return LocalizeOr(key, fallback);
        }
        #endregion
    }
}

  

