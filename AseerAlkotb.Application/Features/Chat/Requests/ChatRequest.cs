using System.Collections.Generic;

namespace AseerAlkotb.Application.Features.Chat.Requests
{
    public class ChatRequest
    {
        public string Question { get; set; }
        public string? Language { get; set; }
        public string? Category { get; set; }
        public int? Limit { get; set; }
    }
}



