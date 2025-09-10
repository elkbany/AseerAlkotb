namespace AseerAlkotb.Dashboard.Models.Embeddings
{
    public class EmbeddingsIndexVM
    {
        public string? Query { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public List<EmbeddingBookRowVM> Rows { get; set; } = new();
    }
}
