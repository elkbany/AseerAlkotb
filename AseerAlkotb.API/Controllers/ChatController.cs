using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Chat.Requests;
using Microsoft.AspNetCore.Http;

namespace AseerAlkotb.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService chatService;
        private readonly IChatLogStore chatLogStore;

        public ChatController(IChatService chatService, IChatLogStore chatLogStore)
        {
            this.chatService = chatService;
            this.chatLogStore = chatLogStore;
        }

        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            var result = await chatService.AskAsync(request);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("log")]
        public IActionResult Log([FromBody] ChatLogRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserMessage))
            {
                return BadRequest(new { message = "Invalid log payload" });
            }
            chatLogStore.Append(request);
            return Ok(new { succeeded = true });
        }
    }
}


