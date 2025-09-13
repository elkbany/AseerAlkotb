using AseerAlkotb.Domain.Entites.Base;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class BookEmbedding : Entity<int>
    {
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
        public string Content { get; set; } = "";
        public string ContentType { get; set; } = "";
        public int? ChunkIndex { get; set; }
        public int? TokenCount { get; set; }
        public float[] Embedding { get; set; } = Array.Empty<float>();
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
