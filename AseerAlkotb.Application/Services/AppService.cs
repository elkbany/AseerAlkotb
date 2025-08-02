using AseerAlkotb.Domain.Entites.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.AspNetCore.Hosting;  // For IWebHostEnvironment
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
namespace AseerAlkotb.Application.Services
{
    public abstract class AppService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IHostEnvironment _environment;
       

        protected AppService(IServiceProvider serviceProvider, IHostEnvironment environment)
        {
            this.serviceProvider = serviceProvider;
            _environment = environment;
          
        }
        #region Validate Async
        protected async Task DoValidationAsync<TValidator, TRequest>(TRequest request, params object[] constructorParameters)
        where TValidator : AbstractValidator<TRequest>
        {
            //var validator = serviceProvider.GetRequiredService<TValidator>();
            //var result = await validator.ValidateAsync(request);

            //if (!result.IsValid)
            //{
            //    throw new ValidationException(result.Errors);
            //}

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

        //public bool IsValidImage(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return false;

        //    // Check file size
        //    if (file.Length > )
        //        return false;

        //    // Check extension
        //    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        //    if (!_allowedExtensions.Contains(extension))
        //        return false;

        //    // Check content type
        //    var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        //    if (!allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
        //        return false;

        //    return true;
        //}

        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var uniqueName = $"{Guid.NewGuid()}{extension}";
            return uniqueName;
        }
        #endregion
    }

}

  

