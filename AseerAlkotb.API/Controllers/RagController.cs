using AseerAlkotb.API.Bases;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Rag.Requests;
using AseerAlkotb.Application.Features.Rag.Responses;
using AseerAlkotb.Application.ResponseHandler;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RagController : AppControllerBase
    {
        private readonly IRagService _rag;

        public RagController(IRagService rag)
        {
            _rag = rag;
        }

        /// <summary>
        /// نقطة واحدة لكل الاستخدامات: تلخيص / ترشيحات / توافر / مؤلف / تصنيف.
        /// يدعم الآن الذاكرة المؤقتة للجلسة - أرسل session-id في الهيدر للاستمرارية.
        /// </summary>
        /// <remarks>
        /// أمثلة:
        /// - {"question":"عايز ترشيحات روايات","category":"روايات","limit":8}
        /// - {"question":"هل كتاب «الخيميائي» متاح؟"}
        /// - {"question":"كتب أخرى لنفس المؤلف: نجيب محفوظ"}
        /// - {"question":"عايز ملخص كتاب العادات الذرية"}
        /// 
        /// للاستمرارية في المحادثة، أرسل session-id في الهيدر:
        /// Headers: { "X-Session-Id": "your-session-id" }
        /// </remarks>
        [HttpPost("ask")]
        [ProducesResponseType(typeof(ApiResponse<RagAskResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<RagAskResponse>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<RagAskResponse>), (int)HttpStatusCode.ServiceUnavailable)]
        [ProducesResponseType(typeof(ApiResponse<RagAskResponse>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Ask([FromBody] RagAskRequest request)
        {
            try
            {
                // Check for session ID in headers for conversation continuity
                string? sessionId = Request.Headers["X-Session-Id"].FirstOrDefault();
                
                // Generate session ID if not provided
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    sessionId = Guid.NewGuid().ToString();
                }
                
                // Always use session-aware method
                var res = await _rag.AskWithSessionAsync(request, sessionId);
                
                // Add session ID to response headers for client to use in subsequent requests
                Response.Headers["X-Session-Id"] = sessionId;
                
                return ApiResult(res);
            }
            catch (ArgumentException ex)
            {
                // أخطاء إدخال/فاليديشِن
                var msg = $"واضح إن في مدخل ناقص أو غير صحيح: {ex.Message}";
                return BadRequest(ApiResponseHandler.BadRequest<RagAskResponse>(msg));
            }
            catch (HttpRequestException ex)
            {
                // مشاكل الاتصال بخدمة خارجية (زي Gemini)
                var msg = $"الخدمة الخارجية متعبة حاليًا. جرّب بعد لحظات. التفاصيل: {ex.Message}";
                return StatusCode(
                    (int)HttpStatusCode.ServiceUnavailable,
                    new ApiResponse<RagAskResponse>
                    {
                        StatusCode = HttpStatusCode.ServiceUnavailable,
                        Message = msg
                    }
                );
            }
            catch (TaskCanceledException ex)
            {
                // Timeout
                var msg = $"انتهت مهلة الطلب أثناء المعالجة. حاول تبسيط السؤال أو أعد المحاولة. التفاصيل: {ex.Message}";
                return StatusCode(
                    (int)HttpStatusCode.ServiceUnavailable,
                    new ApiResponse<RagAskResponse>
                    {
                        StatusCode = HttpStatusCode.ServiceUnavailable,
                        Message = msg
                    }
                );
            }
            catch (Exception ex)
            {
                // أي خطأ غير متوقع
                var msg = $"حصل خطأ غير متوقّع. جرّب تكتب سؤالك بشكل أبسط. التفاصيل: {ex.Message}";
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError,
                    new ApiResponse<RagAskResponse>
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        Message = msg
                    }
                );
            }
        }
    }
}
