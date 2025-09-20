using System;
using System.Collections.Generic;
using System.Linq;

namespace AseerAlkotb.Application.Features.Rag.Models
{
    /// <summary>
    /// Session memory that stores conversation history temporarily during the session
    /// </summary>
    public class SessionMemory
    {
        public string SessionId { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
        public List<SessionMessage> Messages { get; set; } = new();
        public int MaxMessages { get; set; } = 20; // Limit conversation history
        
        /// <summary>
        /// Add a new message to the session
        /// </summary>
        public void AddMessage(SessionMessage message)
        {
            Messages.Add(message);
            LastAccessedAt = DateTime.UtcNow;
            
            // Keep only the latest messages to prevent memory bloat
            if (Messages.Count > MaxMessages)
            {
                Messages = Messages.Skip(Messages.Count - MaxMessages).ToList();
            }
        }
        
        /// <summary>
        /// Get recent messages for context
        /// </summary>
        public List<SessionMessage> GetRecentMessages(int count = 5)
        {
            return Messages.TakeLast(count).ToList();
        }
        
        /// <summary>
        /// Find previous similar questions
        /// </summary>
        public List<SessionMessage> FindSimilarQuestions(string currentQuestion, int maxResults = 3)
        {
            var currentLower = currentQuestion.ToLower();
            return Messages
                .Where(m => !string.IsNullOrEmpty(m.Question))
                .Where(m => 
                {
                    var questionLower = m.Question.ToLower();
                    // Simple similarity check - you could enhance this with more sophisticated matching
                    return questionLower.Contains(currentLower) || 
                           currentLower.Contains(questionLower) ||
                           HasCommonWords(questionLower, currentLower, minCommonWords: 2);
                })
                .OrderByDescending(m => m.Timestamp)
                .Take(maxResults)
                .ToList();
        }
        
        /// <summary>
        /// Check if session has expired (inactive for too long)
        /// </summary>
        public bool IsExpired(TimeSpan sessionTimeout)
        {
            return DateTime.UtcNow - LastAccessedAt > sessionTimeout;
        }
        
        private static bool HasCommonWords(string text1, string text2, int minCommonWords)
        {
            var words1 = text1.Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(w => w.Length > 2).ToHashSet();
            var words2 = text2.Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(w => w.Length > 2).ToHashSet();
            
            return words1.Intersect(words2).Count() >= minCommonWords;
        }
        
        /// <summary>
        /// Get the most recently mentioned author from history
        /// </summary>
        public string? GetLastMentionedAuthor()
        {
            return Messages
                .Where(m => !string.IsNullOrWhiteSpace(m.ExtractedAuthor) || 
                           !string.IsNullOrWhiteSpace(m.EntityCache.NormalizedAuthor))
                .OrderByDescending(m => m.Timestamp)
                .Select(m => m.ExtractedAuthor ?? m.EntityCache.NormalizedAuthor)
                .FirstOrDefault();
        }
        
        /// <summary>
        /// Get the most recently mentioned book title from history
        /// </summary>
        public string? GetLastMentionedTitle()
        {
            return Messages
                .Where(m => !string.IsNullOrWhiteSpace(m.ExtractedTitle) || 
                           !string.IsNullOrWhiteSpace(m.EntityCache.NormalizedTitle))
                .OrderByDescending(m => m.Timestamp)
                .Select(m => m.ExtractedTitle ?? m.EntityCache.NormalizedTitle)
                .FirstOrDefault();
        }
        
        /// <summary>
        /// Get the most recently mentioned publisher from history
        /// </summary>
        public string? GetLastMentionedPublisher()
        {
            return Messages
                .Where(m => !string.IsNullOrWhiteSpace(m.ExtractedPublisher) || 
                           !string.IsNullOrWhiteSpace(m.EntityCache.NormalizedPublisher))
                .OrderByDescending(m => m.Timestamp)
                .Select(m => m.ExtractedPublisher ?? m.EntityCache.NormalizedPublisher)
                .FirstOrDefault();
        }
        
        /// <summary>
        /// Get the most recently mentioned category from history
        /// </summary>
        public string? GetLastMentionedCategory()
        {
            return Messages
                .Where(m => !string.IsNullOrWhiteSpace(m.ExtractedCategory) || 
                           !string.IsNullOrWhiteSpace(m.EntityCache.NormalizedCategory))
                .OrderByDescending(m => m.Timestamp)
                .Select(m => m.ExtractedCategory ?? m.EntityCache.NormalizedCategory)
                .FirstOrDefault();
        }
        
        /// <summary>
        /// Get resolved entity IDs from the most recent relevant message
        /// </summary>
        public (int? bookId, int? authorId, int? publisherId, int? categoryId) GetLastResolvedEntityIds()
        {
            var lastMessage = Messages
                .Where(m => m.EntityCache.ResolvedBookId.HasValue || 
                           m.EntityCache.ResolvedAuthorId.HasValue ||
                           m.EntityCache.ResolvedPublisherId.HasValue ||
                           m.EntityCache.ResolvedCategoryId.HasValue)
                .OrderByDescending(m => m.Timestamp)
                .FirstOrDefault();
                
            return lastMessage != null 
                ? (lastMessage.EntityCache.ResolvedBookId, 
                   lastMessage.EntityCache.ResolvedAuthorId,
                   lastMessage.EntityCache.ResolvedPublisherId,
                   lastMessage.EntityCache.ResolvedCategoryId)
                : (null, null, null, null);
        }
    }
}