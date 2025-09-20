using System.Collections.Concurrent;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Rag.Models;
using AseerAlkotb.Application.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AseerAlkotb.Application.Services
{
    /// <summary>
    /// In-memory session management service for conversation history
    /// Session data is temporary and cleared when application restarts
    /// </summary>
    public class SessionMemoryService : ISessionMemoryService
    {
        private readonly ConcurrentDictionary<string, SessionMemory> _sessions = new();
        private readonly ILogger<SessionMemoryService> _logger;
        private readonly IConfiguration _config;
        private readonly TimeSpan _sessionTimeout;
        
        public SessionMemoryService(ILogger<SessionMemoryService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            
            // Configure session timeout (default: 30 minutes)
            var timeoutMinutes = _config.GetValue<int>("SessionMemory:TimeoutMinutes", 30);
            _sessionTimeout = TimeSpan.FromMinutes(timeoutMinutes);
            
            _logger.LogInformation("SessionMemoryService initialized with {TimeoutMinutes} minutes timeout", timeoutMinutes);
        }

        public async Task<SessionMemory> GetOrCreateSessionAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                sessionId = Guid.NewGuid().ToString();

            var session = _sessions.GetOrAdd(sessionId, _ => new SessionMemory 
            { 
                SessionId = sessionId,
                MaxMessages = _config.GetValue<int>("SessionMemory:MaxMessages", 20)
            });
            
            session.LastAccessedAt = DateTime.UtcNow;
            
            _logger.LogDebug("Session {SessionId} accessed with {MessageCount} messages", 
                sessionId, session.Messages.Count);
            
            return await Task.FromResult(session);
        }

        public async Task AddMessageAsync(string sessionId, SessionMessage message)
        {
            var session = await GetOrCreateSessionAsync(sessionId);
            session.AddMessage(message);
            
            _logger.LogDebug("Added message to session {SessionId}. Total messages: {MessageCount}", 
                sessionId, session.Messages.Count);
        }
        
        public async Task AddMessageWithEntitiesAsync(string sessionId, SessionMessage message, 
            int? resolvedBookId = null, int? resolvedAuthorId = null, 
            int? resolvedPublisherId = null, int? resolvedCategoryId = null,
            string? normalizedTitle = null, string? normalizedAuthor = null,
            string? normalizedPublisher = null, string? normalizedCategory = null)
        {
            // Populate entity cache with resolved information
            message.EntityCache.ResolvedBookId = resolvedBookId;
            message.EntityCache.ResolvedAuthorId = resolvedAuthorId;
            message.EntityCache.ResolvedPublisherId = resolvedPublisherId;
            message.EntityCache.ResolvedCategoryId = resolvedCategoryId;
            message.EntityCache.NormalizedTitle = normalizedTitle;
            message.EntityCache.NormalizedAuthor = normalizedAuthor;
            message.EntityCache.NormalizedPublisher = normalizedPublisher;
            message.EntityCache.NormalizedCategory = normalizedCategory;
            
            await AddMessageAsync(sessionId, message);
            
            _logger.LogDebug("Added message with entity cache to session {SessionId}. Book: {BookId}, Author: {AuthorId}, Publisher: {PublisherId}", 
                sessionId, resolvedBookId, resolvedAuthorId, resolvedPublisherId);
        }

        public async Task<string> GetConversationContextAsync(string sessionId, string currentQuestion)
        {
            var session = await GetOrCreateSessionAsync(sessionId);
            var recentMessages = session.GetRecentMessages(3); // Get last 3 messages for context
            
            if (!recentMessages.Any())
                return string.Empty;

            var contextParts = new List<string>();
            
            // Check for conversation continuity
            var lang = LangUtils.Detect(currentQuestion);
            var contextHeader = lang == Lang.English 
                ? "Based on our previous conversation:" 
                : "بناءً على محادثتنا السابقة:";
                
            contextParts.Add(contextHeader);
            
            foreach (var msg in recentMessages.TakeLast(2)) // Last 2 for brevity
            {
                if (lang == Lang.English)
                {
                    contextParts.Add($"- You asked about: {msg.Question}");
                    if (!string.IsNullOrEmpty(msg.ExtractedTitle))
                        contextParts.Add($"- We discussed the book: {msg.ExtractedTitle}");
                    if (!string.IsNullOrEmpty(msg.ExtractedAuthor))
                        contextParts.Add($"- We talked about author: {msg.ExtractedAuthor}");
                }
                else
                {
                    contextParts.Add($"- سألت عن: {msg.Question}");
                    if (!string.IsNullOrEmpty(msg.ExtractedTitle))
                        contextParts.Add($"- تحدثنا عن كتاب: {msg.ExtractedTitle}");
                    if (!string.IsNullOrEmpty(msg.ExtractedAuthor))
                        contextParts.Add($"- تكلمنا عن المؤلف: {msg.ExtractedAuthor}");
                }
            }
            
            return string.Join("\n", contextParts);
        }

        public async Task<List<SessionMessage>> FindSimilarQuestionsAsync(string sessionId, string currentQuestion)
        {
            var session = await GetOrCreateSessionAsync(sessionId);
            return session.FindSimilarQuestions(currentQuestion);
        }
        
        public async Task<string?> GetCachedAuthorAsync(string sessionId)
        {
            var session = await GetOrCreateSessionAsync(sessionId);
            var author = session.GetLastMentionedAuthor();
            
            if (!string.IsNullOrWhiteSpace(author))
            {
                _logger.LogDebug("Retrieved cached author {Author} from session {SessionId}", author, sessionId);
            }
            
            return author;
        }
        
        public async Task<string?> GetCachedTitleAsync(string sessionId)
        {
            var session = await GetOrCreateSessionAsync(sessionId);
            var title = session.GetLastMentionedTitle();
            
            if (!string.IsNullOrWhiteSpace(title))
            {
                _logger.LogDebug("Retrieved cached title {Title} from session {SessionId}", title, sessionId);
            }
            
            return title;
        }
        
        public async Task<string?> GetCachedPublisherAsync(string sessionId)
        {
            var session = await GetOrCreateSessionAsync(sessionId);
            var publisher = session.GetLastMentionedPublisher();
            
            if (!string.IsNullOrWhiteSpace(publisher))
            {
                _logger.LogDebug("Retrieved cached publisher {Publisher} from session {SessionId}", publisher, sessionId);
            }
            
            return publisher;
        }
        
        public async Task<string?> GetCachedCategoryAsync(string sessionId)
        {
            var session = await GetOrCreateSessionAsync(sessionId);
            var category = session.GetLastMentionedCategory();
            
            if (!string.IsNullOrWhiteSpace(category))
            {
                _logger.LogDebug("Retrieved cached category {Category} from session {SessionId}", category, sessionId);
            }
            
            return category;
        }
        
        public async Task<(int? bookId, int? authorId, int? publisherId, int? categoryId)> GetCachedEntityIdsAsync(string sessionId)
        {
            var session = await GetOrCreateSessionAsync(sessionId);
            return session.GetLastResolvedEntityIds();
        }

        public async Task CleanupExpiredSessionsAsync()
        {
            var expiredSessions = _sessions
                .Where(kvp => kvp.Value.IsExpired(_sessionTimeout))
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var sessionId in expiredSessions)
            {
                _sessions.TryRemove(sessionId, out _);
            }

            if (expiredSessions.Any())
            {
                _logger.LogInformation("Cleaned up {ExpiredCount} expired sessions", expiredSessions.Count);
            }

            await Task.CompletedTask;
        }

        public async Task ClearAllSessionsAsync()
        {
            var sessionCount = _sessions.Count;
            _sessions.Clear();
            
            _logger.LogInformation("Cleared all {SessionCount} sessions", sessionCount);
            await Task.CompletedTask;
        }
        
        // Background cleanup method (can be called periodically)
        public void StartPeriodicCleanup()
        {
            var timer = new Timer(async _ => await CleanupExpiredSessionsAsync(), 
                null, 
                TimeSpan.FromMinutes(10), // Start after 10 minutes
                TimeSpan.FromMinutes(15)  // Run every 15 minutes
            );
        }
    }
}