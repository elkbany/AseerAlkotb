using System;
using System.Collections.Generic;
using AseerAlkotb.Application.Features.Chat.Responses;

namespace AseerAlkotb.Application.Features.Chat.Requests
{
    public class ChatLogRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string UserMessage { get; set; } = string.Empty;
        public ChatResponse AiResponse { get; set; } = new ChatResponse();
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    }
}



