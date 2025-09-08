using AseerAlkotb.Domain.Entites.Base;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class BookEmbedding : Entity<int>
    {
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
        
        public string Content { get; set; } // النص الذي تم تحويله إلى embedding
        public string ContentType { get; set; } // "title", "description", "author", "category"
        public float[] Embedding { get; set; } // Vector representation
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
