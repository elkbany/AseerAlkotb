using System;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Rag.Models;
using AseerAlkotb.Application.Features.Rag.Requests;
using AseerAlkotb.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AseerAlkotb.Tests
{
    /// <summary>
    /// Test class for session entity caching functionality
    /// </summary>
    public class TestSessionEntityCaching
    {
        public static async Task<bool> RunTestsAsync()
        {
            Console.WriteLine("=== Testing Session Entity Caching ===");
            
            // Setup test dependencies
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"SessionMemory:TimeoutMinutes", "30"},
                    {"SessionMemory:MaxMessages", "20"}
                })
                .Build());
            
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<SessionMemoryService>>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            
            var sessionMemoryService = new SessionMemoryService(logger, config);
            
            bool allTestsPassed = true;
            
            // Test 1: Basic entity caching
            Console.WriteLine("\n--- Test 1: Basic Entity Caching ---");
            try
            {
                var sessionId = Guid.NewGuid().ToString();
                
                // Add a message with author information
                var message1 = new SessionMessage
                {
                    Question = "ما هي كتب نجيب محفوظ؟",
                    Answer = "نجيب محفوظ له كتب كثيرة منها الثلاثية وأولاد حارتنا",
                    Intent = "more_by_author",
                    ExtractedAuthor = "نجيب محفوظ"
                };
                
                await sessionMemoryService.AddMessageWithEntitiesAsync(
                    sessionId, message1,
                    resolvedAuthorId: 123,
                    normalizedAuthor: "نجيب محفوظ"
                );
                
                // Test retrieving cached author
                var cachedAuthor = await sessionMemoryService.GetCachedAuthorAsync(sessionId);
                if (cachedAuthor == "نجيب محفوظ")
                {
                    Console.WriteLine("✓ Successfully cached and retrieved author");
                }
                else
                {
                    Console.WriteLine($"✗ Failed to cache author. Expected: 'نجيب محفوظ', Got: '{cachedAuthor}'");
                    allTestsPassed = false;
                }
                
                // Test entity IDs
                var (bookId, authorId, publisherId, categoryId) = await sessionMemoryService.GetCachedEntityIdsAsync(sessionId);
                if (authorId == 123)
                {
                    Console.WriteLine("✓ Successfully cached and retrieved author ID");
                }
                else
                {
                    Console.WriteLine($"✗ Failed to cache author ID. Expected: 123, Got: {authorId}");
                    allTestsPassed = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Test 1 failed with exception: {ex.Message}");
                allTestsPassed = false;
            }
            
            // Test 2: Multiple entity types
            Console.WriteLine("\n--- Test 2: Multiple Entity Types ---");
            try
            {
                var sessionId = Guid.NewGuid().ToString();
                
                // Add message with book and publisher info
                var message2 = new SessionMessage
                {
                    Question = "أريد معلومات عن كتاب الخيميائي",
                    Answer = "الخيميائي كتاب رائع من دار الشروق",
                    Intent = "summary",
                    ExtractedTitle = "الخيميائي",
                    ExtractedPublisher = "دار الشروق"
                };
                
                await sessionMemoryService.AddMessageWithEntitiesAsync(
                    sessionId, message2,
                    resolvedBookId: 456,
                    resolvedPublisherId: 789,
                    normalizedTitle: "الخيميائي",
                    normalizedPublisher: "دار الشروق"
                );
                
                // Test retrieving multiple cached entities
                var cachedTitle = await sessionMemoryService.GetCachedTitleAsync(sessionId);
                var cachedPublisher = await sessionMemoryService.GetCachedPublisherAsync(sessionId);
                
                if (cachedTitle == "الخيميائي" && cachedPublisher == "دار الشروق")
                {
                    Console.WriteLine("✓ Successfully cached multiple entity types");
                }
                else
                {
                    Console.WriteLine($"✗ Failed to cache multiple entities. Title: '{cachedTitle}', Publisher: '{cachedPublisher}'");
                    allTestsPassed = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Test 2 failed with exception: {ex.Message}");
                allTestsPassed = false;
            }
            
            // Test 3: Session context preservation
            Console.WriteLine("\n--- Test 3: Session Context Preservation ---");
            try
            {
                var sessionId = Guid.NewGuid().ToString();
                
                // First message - user asks about an author
                var message1 = new SessionMessage
                {
                    Question = "من هو أحمد خالد توفيق؟",
                    Answer = "أحمد خالد توفيق كاتب مصري مشهور بأدب الرعب والخيال العلمي",
                    Intent = "author_bio",
                    ExtractedAuthor = "أحمد خالد توفيق"
                };
                
                await sessionMemoryService.AddMessageWithEntitiesAsync(
                    sessionId, message1,
                    resolvedAuthorId: 999,
                    normalizedAuthor: "أحمد خالد توفيق"
                );
                
                // Second message - user asks about books (without mentioning author)
                var message2 = new SessionMessage
                {
                    Question = "ما هي أشهر كتبه؟",
                    Answer = "من أشهر كتبه سلسلة ما وراء الطبيعة وسلسلة فانتازيا",
                    Intent = "more_by_author"
                };
                
                await sessionMemoryService.AddMessageAsync(sessionId, message2);
                
                // Test that we can still retrieve the cached author from previous message
                var cachedAuthor = await sessionMemoryService.GetCachedAuthorAsync(sessionId);
                if (cachedAuthor == "أحمد خالد توفيق")
                {
                    Console.WriteLine("✓ Successfully preserved author context across messages");
                }
                else
                {
                    Console.WriteLine($"✗ Failed to preserve context. Expected: 'أحمد خالد توفيق', Got: '{cachedAuthor}'");
                    allTestsPassed = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Test 3 failed with exception: {ex.Message}");
                allTestsPassed = false;
            }
            
            // Test 4: Most recent entity priority
            Console.WriteLine("\n--- Test 4: Most Recent Entity Priority ---");
            try
            {
                var sessionId = Guid.NewGuid().ToString();
                
                // Add first author
                var message1 = new SessionMessage
                {
                    Question = "كتب نجيب محفوظ",
                    ExtractedAuthor = "نجيب محفوظ"
                };
                await sessionMemoryService.AddMessageAsync(sessionId, message1);
                
                // Add second author (more recent)
                var message2 = new SessionMessage
                {
                    Question = "كتب يوسف إدريس",
                    ExtractedAuthor = "يوسف إدريس"
                };
                await sessionMemoryService.AddMessageAsync(sessionId, message2);
                
                // Should return the most recent author
                var cachedAuthor = await sessionMemoryService.GetCachedAuthorAsync(sessionId);
                if (cachedAuthor == "يوسف إدريس")
                {
                    Console.WriteLine("✓ Correctly prioritizes most recent entity");
                }
                else
                {
                    Console.WriteLine($"✗ Failed to prioritize recent entity. Expected: 'يوسف إدريس', Got: '{cachedAuthor}'");
                    allTestsPassed = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Test 4 failed with exception: {ex.Message}");
                allTestsPassed = false;
            }
            
            Console.WriteLine($"\n=== Test Results ===");
            Console.WriteLine($"All tests passed: {allTestsPassed}");
            
            return allTestsPassed;
        }
    }
}

/// <summary>
/// Simple test runner
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var result = await AseerAlkotb.Tests.TestSessionEntityCaching.RunTestsAsync();
            Console.WriteLine($"\nSession Entity Caching Tests: {(result ? "PASSED" : "FAILED")}");
            Environment.Exit(result ? 0 : 1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Test execution failed: {ex.Message}");
            Environment.Exit(1);
        }
    }
}