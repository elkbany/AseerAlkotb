using AseerAlkotb.Application.ResponseHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocalizationController : ControllerBase
    {
        [HttpPost("refresh")]
        public IActionResult RefreshLocalization()
        {
            try
            {
                // إعادة تحميل الـ localization resources
                LocalizerProvider.RefreshLocalizer();
                
                return Ok(new { message = "Localization resources refreshed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to refresh localization resources", error = ex.Message });
            }
        }

        [HttpPost("force-refresh")]
        public IActionResult ForceRefreshLocalization()
        {
            try
            {
                // إعادة تحميل الـ localization resources بقوة
                LocalizerProvider.RefreshLocalizer();
                
                // اختبار الـ localization
                var testValue = LocalizerProvider.GetLocalizedMessage("Category_64_Name");
                
                return Ok(new { 
                    message = "Localization resources refreshed successfully",
                    testValue = testValue,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to refresh localization resources", error = ex.Message });
            }
        }

        [HttpGet("test/{key}")]
        public IActionResult TestLocalization(string key)
        {
            try
            {
                var localizedValue = LocalizerProvider.GetLocalizedMessage(key);
                return Ok(new { key = key, value = localizedValue });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to get localized value", error = ex.Message });
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}
