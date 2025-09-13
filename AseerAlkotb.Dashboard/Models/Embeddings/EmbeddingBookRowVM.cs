namespace AseerAlkotb.Dashboard.Models.Embeddings
{
    public class EmbeddingBookRowVM
    {
        public int BookId { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Categories { get; set; } = "";
        public DateTime? LastUpdated { get; set; }
        public bool HasEmbeddings => LastUpdated.HasValue;
    }
}
