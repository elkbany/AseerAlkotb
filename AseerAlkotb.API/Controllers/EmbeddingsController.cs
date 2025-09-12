using AseerAlkotb.API.Bases;
using AseerAlkotb.Application.BackgroundJobs;
using AseerAlkotb.Application.ResponseHandler;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmbeddingsController : AppControllerBase
    {
        private readonly IEmbeddingRefreshJob _job;

        public EmbeddingsController(IEmbeddingRefreshJob job)
        {
            _job = job;
        }

        /// <summary>ابدأ تحديث لكل الكتب في الخلفية.</summary>
        [HttpPost("refresh-all")]
        public IActionResult RefreshAll()
        {
            var enqueued = _job.TriggerFullRebuild();
            return Ok(ApiResponseHandler.Success(new { enqueued }));
        }

        /// <summary>حدّث كتاب واحد بالـ Id في الخلفية.</summary>
        [HttpPost("refresh-one/{bookId:int}")]
        public IActionResult RefreshOne([FromRoute] int bookId)
        {
            var enqueued = _job.TriggerBookUpdate(bookId);
            return Ok(ApiResponseHandler.Success(new { enqueued, bookId }));
        }

        /// <summary>حالة الشغل الحالي/الأخير.</summary>
        [HttpGet("status")]
        public IActionResult Status()
        {
            var s = _job.GetStatus();
            return Ok(ApiResponseHandler.Success(s));
        }
    }
}
