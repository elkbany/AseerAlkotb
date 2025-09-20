using AseerAlkotb.Application.Features.Rag.Models;

namespace AseerAlkotb.Application.Contracts
{
    /// <summary>
    /// Interface for managing session-based conversation memory
    /// </summary>
    public interface ISessionMemoryService
    {
        /// <summary>
        /// Get or create a session memory for the given session ID
        /// </summary>
        Task<SessionMemory> GetOrCreateSessionAsync(string sessionId);
        
        /// <summary>
        /// Add a message to the session
        /// </summary>
        Task AddMessageAsync(string sessionId, SessionMessage message);
        
        /// <summary>
        /// Add a message to the session with resolved entity information
        /// </summary>
        Task AddMessageWithEntitiesAsync(string sessionId, SessionMessage message, 
            int? resolvedBookId = null, int? resolvedAuthorId = null, 
            int? resolvedPublisherId = null, int? resolvedCategoryId = null,
            string? normalizedTitle = null, string? normalizedAuthor = null,
            string? normalizedPublisher = null, string? normalizedCategory = null);
        
        /// <summary>
        /// Get conversation context for better responses
        /// </summary>
        Task<string> GetConversationContextAsync(string sessionId, string currentQuestion);
        
        /// <summary>
        /// Check if user has asked similar questions before
        /// </summary>
        Task<List<SessionMessage>> FindSimilarQuestionsAsync(string sessionId, string currentQuestion);
        
        /// <summary>
        /// Get cached author name from session history
        /// </summary>
        Task<string?> GetCachedAuthorAsync(string sessionId);
        
        /// <summary>
        /// Get cached book title from session history
        /// </summary>
        Task<string?> GetCachedTitleAsync(string sessionId);
        
        /// <summary>
        /// Get cached publisher name from session history
        /// </summary>
        Task<string?> GetCachedPublisherAsync(string sessionId);
        
        /// <summary>
        /// Get cached category name from session history
        /// </summary>
        Task<string?> GetCachedCategoryAsync(string sessionId);
        
        /// <summary>
        /// Get cached resolved entity IDs from session history
        /// </summary>
        Task<(int? bookId, int? authorId, int? publisherId, int? categoryId)> GetCachedEntityIdsAsync(string sessionId);
        
        /// <summary>
        /// Clean up expired sessions
        /// </summary>
        Task CleanupExpiredSessionsAsync();
        
        /// <summary>
        /// Clear all session data (for testing or maintenance)
        /// </summary>
        Task ClearAllSessionsAsync();
    }
}