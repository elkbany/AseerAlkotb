using System;

namespace AseerAlkotb.Application.Features.Rag.Models
{
    /// <summary>
    /// Represents a single message in the conversation session
    /// </summary>
    public class SessionMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Question { get; set; } = "";
        public string Answer { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Intent { get; set; }
        public string? ExtractedTitle { get; set; }
        public string? ExtractedAuthor { get; set; }
        public string? ExtractedCategory { get; set; }
        public string? ExtractedPublisher { get; set; }
        public bool IsEnglishQuery { get; set; }
        
        /// <summary>
        /// Additional metadata about resolved entities for this message
        /// </summary>
        public SessionEntityCache EntityCache { get; set; } = new();
    }
    
    /// <summary>
    /// Cache for resolved entities in the session message
    /// </summary>
    public class SessionEntityCache
    {
        /// <summary>
        /// Book ID if a specific book was referenced/found
        /// </summary>
        public int? ResolvedBookId { get; set; }
        
        /// <summary>
        /// Author ID if a specific author was referenced/found
        /// </summary>
        public int? ResolvedAuthorId { get; set; }
        
        /// <summary>
        /// Publisher ID if a specific publisher was referenced/found
        /// </summary>
        public int? ResolvedPublisherId { get; set; }
        
        /// <summary>
        /// Category ID if a specific category was referenced/found
        /// </summary>
        public int? ResolvedCategoryId { get; set; }
        
        /// <summary>
        /// Normalized/cleaned entity names for better matching
        /// </summary>
        public string? NormalizedTitle { get; set; }
        public string? NormalizedAuthor { get; set; }
        public string? NormalizedPublisher { get; set; }
        public string? NormalizedCategory { get; set; }
    }
}