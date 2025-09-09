namespace AseerAlkotb.Application.Features.Rag.Responses
{
    public class RagAskResponse
    {
        public string Answer { get; set; } = "";
        public bool? IsAvailable { get; set; }
        public int? PrimaryBookId { get; set; }
        public string? PrimaryBookTitle { get; set; }
        public bool CanAddToCart { get; set; }
        public List<ChatSource> Sources { get; set; } = new();
    }
}
