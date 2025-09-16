﻿﻿using AseerAlkotb.Application.Contracts.External;
using AseerAlkotb.Application.Features.UploadImages.Dto;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Localization.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
//using Microsoft.AspNetCore.Hosting;  // For IWebHostEnvironment
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using System;
using System.Globalization;

namespace AseerAlkotb.Application.Services
{
    public abstract class AppService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IHostEnvironment _environment;
        protected readonly ICloudinaryService? _cloudinaryService;


        private IStringLocalizer<SharedResources>? _localizer;

        protected IStringLocalizer<SharedResources> _stringLocalizer =>
            _localizer ??= serviceProvider.GetRequiredService<IStringLocalizer<SharedResources>>();

        protected AppService(IServiceProvider serviceProvider, IHostEnvironment environment)
        {
            this.serviceProvider = serviceProvider;
            _environment = environment;
            _cloudinaryService = serviceProvider.GetService<ICloudinaryService>();
        }

        #region Validate Async
        protected async Task DoValidationAsync<TValidator, TRequest>(TRequest request)
            where TValidator : AbstractValidator<TRequest>
        {
            // Use DI container instead of Activator.CreateInstance
            var validator = serviceProvider.GetRequiredService<TValidator>();
            var validateResult = await validator.ValidateAsync(request);
            if (!validateResult.IsValid)
            {
                throw new ValidationException(validateResult.Errors);
            }
        }
        #endregion

        #region Uploading Files
        public async Task<UploadResultDto> UploadImageAsync(IFormFile imageFile, string folder)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                    throw new ArgumentException("No image file provided");

                var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
                if (!allowedContentTypes.Contains(imageFile.ContentType.ToLowerInvariant()))
                    throw new ArgumentException("Invalid image file type");

                // ✅ Cloudinary Upload only
                if (_cloudinaryService == null)
                    throw new InvalidOperationException("Cloudinary service is not available");

                string cloudUrl = await _cloudinaryService.UploadImageAsync(imageFile, folder);

                // ✅ Return Cloudinary URL only
                return new UploadResultDto
                {
                    CloudUrl = cloudUrl
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UploadImageAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(string localImageUrl, string? cloudPublicId = null)
        {
            var deletedLocal = false;



            // ✅ Delete Cloud
            var deletedCloud = true;
            if (!string.IsNullOrEmpty(cloudPublicId))
            {
                deletedCloud = await _cloudinaryService.DeleteImageAsync(cloudPublicId);
            }

            return deletedLocal && deletedCloud;
        }

        public async Task<UploadResultDto> UpdateImageAsync(IFormFile newImage, string? oldImageUrl, string folder)
        {
            try
            {
                // Upload الجديدة
                var newUpload = await UploadImageAsync(newImage, folder);

                // استخرج الـ public ID من الـ URL القديمة وامسحها (إذا كان Cloudinary متاح)
                if (!string.IsNullOrEmpty(oldImageUrl) && _cloudinaryService != null)
                {
                    try
                    {
                        var oldCloudPublicId = ExtractPublicIdFromUrl(oldImageUrl);
                        if (!string.IsNullOrEmpty(oldCloudPublicId))
                        {
                            await DeleteImageAsync(oldImageUrl, oldCloudPublicId);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Failed to delete old image: {ex.Message}");
                    }
                }

                return newUpload;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateImageAsync: {ex.Message}");
                throw;
            }
        }

        private string? ExtractPublicIdFromUrl(string cloudinaryUrl)
        {
            if (string.IsNullOrEmpty(cloudinaryUrl) || !cloudinaryUrl.Contains("cloudinary.com"))
                return null;

            var parts = cloudinaryUrl.Split('/');
            var uploadIndex = Array.IndexOf(parts, "upload");

            if (uploadIndex >= 0 && uploadIndex + 2 < parts.Length)
            {
                var publicIdParts = parts.Skip(uploadIndex + 2).ToArray();
                var lastPart = publicIdParts.Last();
                publicIdParts[publicIdParts.Length - 1] = Path.GetFileNameWithoutExtension(lastPart);
                return string.Join("/", publicIdParts);
            }

            return null;
        }



        //private string GenerateUniqueFileName(string originalFileName)
        //{
        //    var extension = Path.GetExtension(originalFileName);
        //    var uniqueName = $"{Guid.NewGuid()}{extension}";
        //    return uniqueName;
        //}
        #endregion

        #region Localization Helpers
        protected string LocalizeOr(string key, string fallback)
        {
            // 1) Try reading the runtime-updated resx on disk (no restart needed)
            try
            {
                var culture = CultureInfo.CurrentUICulture?.TwoLetterISOLanguageName ?? "en";
                var direct = ResxResourceHelper.GetSharedResourceOrNull(key, culture);
                if (!string.IsNullOrWhiteSpace(direct))
                {
                    return direct!;
                }
            }
            catch { /* ignore and fallback */ }

            // 2) Fallback to the registered IStringLocalizer (may be cached)
            var localized = _stringLocalizer[key];
            if (!localized.ResourceNotFound && !string.IsNullOrWhiteSpace(localized.Value))
            {
                return localized.Value;
            }

            // 3) Final fallback to provided default value
            return fallback;
        }

        protected string LocalizeEntity(string entityType, int id, string field, string fallback)
        {
            var key = $"{entityType}_{id}_{field}";
            return LocalizeOr(key, fallback);
        }

        /// <summary>
        /// Gets localized text based on current culture using entity English fields
        /// Returns Arabic field for 'ar' culture, English field for other cultures
        /// Falls back to Arabic if English is not available
        /// </summary>
        protected string GetLocalizedText(string arabicText, string? englishText)
        {
            // Try to get culture from multiple sources for better reliability
            var culture = GetCurrentCulture();
            
            if (culture == "ar")
            {
                return arabicText ?? string.Empty;
            }
            
            // For English or other cultures, return English if available, otherwise Arabic
            return !string.IsNullOrWhiteSpace(englishText) ? englishText : arabicText ?? string.Empty;
        }

        /// <summary>
        /// Gets the current culture from multiple sources with fallback logic
        /// </summary>
        private string GetCurrentCulture()
        {
            try
            {
                // 1. Try to get from HttpContext first (most reliable for web requests)
                var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
                if (httpContextAccessor?.HttpContext != null)
                {
                    var requestCulture = httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>();
                    if (requestCulture != null)
                    {
                        return requestCulture.RequestCulture.Culture.TwoLetterISOLanguageName;
                    }
                }

                // 2. Fallback to Thread culture
                var culture = CultureInfo.CurrentUICulture?.TwoLetterISOLanguageName;
                if (!string.IsNullOrEmpty(culture))
                {
                    return culture;
                }
            }
            catch
            {
                // Ignore errors and use fallback
            }

            // 3. Final fallback
            return "en";
        }
        #endregion
    }
}

  

