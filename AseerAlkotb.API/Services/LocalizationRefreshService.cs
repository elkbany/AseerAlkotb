using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Localization.Resources;
using System.IO;

namespace AseerAlkotb.API.Services
{
    public class LocalizationRefreshService : BackgroundService
    {
        private readonly ILogger<LocalizationRefreshService> _logger;
        private FileSystemWatcher? _watcher;

        public LocalizationRefreshService(ILogger<LocalizationRefreshService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // البحث عن مجلد الـ localization
                var localizationPath = FindLocalizationPath();
                if (string.IsNullOrEmpty(localizationPath))
                {
                    _logger.LogWarning("Localization path not found");
                    return;
                }

                _watcher = new FileSystemWatcher(localizationPath)
                {
                    Filter = "*.resx",
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = false
                };

                _watcher.Changed += OnFileChanged;
                _watcher.Created += OnFileChanged;
                _watcher.Renamed += OnFileChanged;

                _logger.LogInformation($"Watching localization files in: {localizationPath}");

                // انتظار حتى يتم إلغاء الـ cancellation token
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LocalizationRefreshService");
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                _logger.LogInformation($"Localization file changed: {e.FullPath}");
                
                // انتظار قصير عشان الـ file يتم حفظه بالكامل
                Task.Delay(500).Wait();
                
                // إعادة تحميل الـ localizer
                LocalizerProvider.RefreshLocalizer();
                
                _logger.LogInformation("Localization resources refreshed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing localization resources");
            }
        }

        private string? FindLocalizationPath()
        {
            try
            {
                var currentDir = Directory.GetCurrentDirectory();
                _logger.LogInformation($"Current directory: {currentDir}");
                
                // البحث عن مجلد AseerAlkotb.Localization
                var searchPaths = new[]
                {
                    Path.Combine(currentDir, "AseerAlkotb.Localization", "Resources"),
                    Path.Combine(currentDir, "..", "AseerAlkotb.Localization", "Resources"),
                    Path.Combine(currentDir, "..", "..", "AseerAlkotb.Localization", "Resources"),
                    Path.Combine(currentDir, "..", "..", "..", "AseerAlkotb.Localization", "Resources"),
                    Path.Combine(currentDir, "..", "..", "..", "..", "AseerAlkotb.Localization", "Resources")
                };

                foreach (var path in searchPaths)
                {
                    _logger.LogInformation($"Checking path: {path}");
                    if (Directory.Exists(path))
                    {
                        _logger.LogInformation($"Found localization path: {path}");
                        return path;
                    }
                }
                
                _logger.LogWarning("Localization path not found in any of the search paths");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding localization path");
            }

            return null;
        }

        public override void Dispose()
        {
            _watcher?.Dispose();
            base.Dispose();
        }
    }
}
