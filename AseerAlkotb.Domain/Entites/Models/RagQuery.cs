namespace AseerAlkotb.Domain.Entites.Models
{
    public class RagQuery
    {
        public int Id { get; set; }
        public string Query { get; set; }
        public string Response { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? UserId { get; set; } // إذا كان النظام يدعم المستخدمين
        public QueryType QueryType { get; set; }
        public double SimilarityScore { get; set; }
    }

    public enum QueryType
    {
        BookSearch,
        AuthorSearch,
        CategorySearch,
        GeneralQuestion,
        Recommendation
    }
}
