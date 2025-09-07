using System.Collections.Generic;

namespace AseerAlkotb.Application.Features.Chat.Responses
{
    public class ChatResponse
    {
        public string Answer { get; set; }
        public List<ChatSource> Sources { get; set; } = new();
        public bool? IsAvailable { get; set; } // if the answer is about a specific book availability
        public int? PrimaryBookId { get; set; }
        public string? PrimaryBookTitle { get; set; }
        public bool? CanAddToCart { get; set; }
    }

    public class ChatSource
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string? Snippet { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}


