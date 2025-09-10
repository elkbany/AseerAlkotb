namespace AseerAlkotb.Dashboard.Models.Embeddings
{
    public class SearchResultVM
    {
        public string Query { get; set; } = "";
        public List<(int BookId, string Title, string MatchType, double Score)> Results { get; set; } = new();
    }
}
