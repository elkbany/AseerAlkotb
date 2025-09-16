using AseerAlkotb.Infrastructure.AI;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace AseerAlkotb.Tests
{
    /// <summary>
    /// Simple test to verify Gemini Router Service works correctly
    /// </summary>
    public class TestGeminiRouter
    {
        public static async Task Main(string[] args)
        {
            // Test questions in Arabic
            var testQuestions = new[]
            {
                "نبذة عن كتاب أولاد حارتنا",
                "كم سعر كتاب العادات الذرية",
                "هل كتاب هاري بوتر متاح؟",
                "نبذة عن نجيب محفوظ",
                "كتب أخرى لنفس مؤلف الخيميائي",
                "رشح كتب شبه أولاد حارتنا",
                "كتب روايات عربية",
                "ترشيحات كتب تطوير ذات"
            };

            // Mock configuration (replace with actual configuration)
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Gemini:ApiKey"] = "your-api-key-here",
                    ["Gemini:Model"] = "gemini-1.5-flash"
                })
                .Build();

            var httpClientFactory = new MockHttpClientFactory();
            var router = new GeminiQuestionRouterService(httpClientFactory, config);

            Console.WriteLine("Testing Gemini Router Service:");
            Console.WriteLine("===================================");

            foreach (var question in testQuestions)
            {
                try
                {
                    Console.WriteLine($"\nQuestion: {question}");
                    var result = await router.RouteAsync(question);
                    
                    Console.WriteLine($"Intent: {result.intent}");
                    Console.WriteLine($"Title: {result.entities.title ?? "null"}");
                    Console.WriteLine($"Author: {result.entities.author ?? "null"}");
                    Console.WriteLine($"Category: {result.entities.category ?? "null"}");
                    Console.WriteLine($"Language: {result.language}");
                    Console.WriteLine($"Confidence: {result.confidence:F2}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }

    // Mock implementation for testing
    public class MockHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            return new HttpClient();
        }
    }
}