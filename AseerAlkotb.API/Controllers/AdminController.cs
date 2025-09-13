using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AseerAlkotb.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public AdminController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("upload-resx")]
        public async Task<IActionResult> UploadResxFile([FromForm] string fileContent, [FromForm] string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileContent) || string.IsNullOrEmpty(fileName))
                {
                    return BadRequest("File content and file name are required");
                }

                // Validate file name
                if (!fileName.EndsWith(".resx"))
                {
                    return BadRequest("Only .resx files are allowed");
                }

                // Get the localization resources path
                var localizationPath = Path.Combine(_environment.ContentRootPath, "..", "AseerAlkotb.Localization", "Resources");
                
                // Ensure directory exists
                Directory.CreateDirectory(localizationPath);

                // Create backup of existing file
                var filePath = Path.Combine(localizationPath, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    var backupPath = Path.Combine(localizationPath, "backup");
                    Directory.CreateDirectory(backupPath);
                    var backupFile = Path.Combine(backupPath, $"{fileName}.backup.{DateTime.Now:yyyyMMddHHmmss}");
                    System.IO.File.Copy(filePath, backupFile);
                }

                // Write new file
                await System.IO.File.WriteAllTextAsync(filePath, fileContent, Encoding.UTF8);

                return Ok(new { message = $"File {fileName} uploaded successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("backup-resx")]
        public IActionResult BackupResxFiles()
        {
            try
            {
                var localizationPath = Path.Combine(_environment.ContentRootPath, "..", "AseerAlkotb.Localization", "Resources");
                var backupPath = Path.Combine(localizationPath, "backup");
                
                Directory.CreateDirectory(backupPath);

                var arabicFile = Path.Combine(localizationPath, "SharedResources.ar.resx");
                var englishFile = Path.Combine(localizationPath, "SharedResources.en.resx");

                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

                if (System.IO.File.Exists(arabicFile))
                {
                    var backupArabic = Path.Combine(backupPath, $"SharedResources.ar.resx.backup.{timestamp}");
                    System.IO.File.Copy(arabicFile, backupArabic);
                }

                if (System.IO.File.Exists(englishFile))
                {
                    var backupEnglish = Path.Combine(backupPath, $"SharedResources.en.resx.backup.{timestamp}");
                    System.IO.File.Copy(englishFile, backupEnglish);
                }

                return Ok(new { message = "Backup created successfully", timestamp });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("restore-resx")]
        public IActionResult RestoreResxFiles()
        {
            try
            {
                var localizationPath = Path.Combine(_environment.ContentRootPath, "..", "AseerAlkotb.Localization", "Resources");
                var backupPath = Path.Combine(localizationPath, "backup");
                
                if (!Directory.Exists(backupPath))
                {
                    return NotFound("No backup directory found");
                }

                var backupFiles = Directory.GetFiles(backupPath, "*.backup.*")
                    .OrderByDescending(f => System.IO.File.GetCreationTime(f))
                    .ToArray();

                if (backupFiles.Length == 0)
                {
                    return NotFound("No backup files found");
                }

                // Restore latest Arabic backup
                var arabicBackup = backupFiles.FirstOrDefault(f => f.Contains("SharedResources.ar.resx"));
                if (arabicBackup != null)
                {
                    var arabicFile = Path.Combine(localizationPath, "SharedResources.ar.resx");
                    System.IO.File.Copy(arabicBackup, arabicFile, true);
                }

                // Restore latest English backup
                var englishBackup = backupFiles.FirstOrDefault(f => f.Contains("SharedResources.en.resx"));
                if (englishBackup != null)
                {
                    var englishFile = Path.Combine(localizationPath, "SharedResources.en.resx");
                    System.IO.File.Copy(englishBackup, englishFile, true);
                }

                return Ok(new { message = "Files restored successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}