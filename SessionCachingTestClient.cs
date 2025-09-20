using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AseerAlkotb.Tests
{
    /// <summary>
    /// Test client to verify session entity caching functionality
    /// </summary>
    public class SessionCachingTestClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public SessionCachingTestClient(string baseUrl = "https://localhost:7207")
        {
            _httpClient = new HttpClient();
            _baseUrl = baseUrl;
        }

        public async Task<bool> RunSessionTestAsync()
        {
            Console.WriteLine("=== Testing Session Entity Caching ===\n");

            try
            {
                // Test 1: Ask about an author (should cache the author)
                Console.WriteLine("🔸 Test 1: Asking about an author...");
                var (response1, sessionId) = await SendRagRequestAsync(new
                {
                    question = "من هو نجيب محفوظ؟",
                    limit = 5
                });

                if (string.IsNullOrEmpty(sessionId))
                {
                    Console.WriteLine("❌ FAILED: No session ID returned in first request");
                    return false;
                }

                Console.WriteLine($"✅ Session ID received: {sessionId}");
                Console.WriteLine($"✅ Response: {response1.data?.answer?[..Math.Min(100, response1.data.answer.Length ?? 0)]}...\n");

                // Test 2: Ask about the author's books using the same session (should use cached author)
                Console.WriteLine("🔸 Test 2: Asking about his books (using cached author)...");
                var (response2, sessionId2) = await SendRagRequestAsync(new
                {
                    question = "ما هي أشهر كتبه؟",
                    limit = 5
                }, sessionId);

                // Verify session persistence
                if (sessionId2 != sessionId)
                {
                    Console.WriteLine("⚠️  WARNING: Session ID changed between requests");
                }

                // Check if the response uses cached context
                if (response2.data?.answer?.Contains("برجاء تحديد اسم المؤلف") == true)
                {
                    Console.WriteLine("❌ FAILED: System asked to specify author name - caching not working");
                    Console.WriteLine($"Response: {response2.data.answer}");
                    return false;
                }

                if (response2.data?.answer?.Contains("نجيب محفوظ") == true || 
                    response2.data?.answer?.Contains("كتب") == true)
                {
                    Console.WriteLine("✅ SUCCESS: System used cached author context!");
                    Console.WriteLine($"✅ Response: {response2.data.answer?[..Math.Min(150, response2.data.answer.Length ?? 0)]}...");
                }
                else
                {
                    Console.WriteLine("⚠️  UNCERTAIN: Response doesn't clearly indicate cached context usage");
                    Console.WriteLine($"Response: {response2.data?.answer}");
                }

                Console.WriteLine("\n=== Session Test Summary ===");
                Console.WriteLine($"✅ Session ID persistence: {(sessionId2 == sessionId ? "PASSED" : "FAILED")}");
                Console.WriteLine($"✅ Entity caching: {(response2.data?.answer?.Contains("برجاء تحديد") != true ? "PASSED" : "FAILED")}");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR: {ex.Message}");
                return false;
            }
        }

        private async Task<(dynamic data, string sessionId)> SendRagRequestAsync(object requestBody, string sessionId = null)
        {
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/Rag/ask")
            {
                Content = content
            };

            // Add session ID header if provided
            if (!string.IsNullOrEmpty(sessionId))
            {
                request.Headers.Add("X-Session-Id", sessionId);
            }

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Extract session ID from response headers
            var responseSessionId = "";
            if (response.Headers.Contains("X-Session-Id"))
            {
                responseSessionId = string.Join("", response.Headers.GetValues("X-Session-Id"));
            }

            Console.WriteLine($"   Status: {response.StatusCode}");
            Console.WriteLine($"   Session ID: {responseSessionId}");

            var responseData = JsonSerializer.Deserialize<dynamic>(responseContent);
            return (responseData, responseSessionId);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    /// <summary>
    /// Simple test runner
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var testClient = new SessionCachingTestClient();
            
            Console.WriteLine("Starting session entity caching test...\n");
            Console.WriteLine("Make sure the AseerAlkotb.API is running on https://localhost:7207\n");

            try
            {
                var success = await testClient.RunSessionTestAsync();
                
                Console.WriteLine($"\n=== Final Result ===");
                Console.WriteLine($"Session Entity Caching Test: {(success ? "✅ PASSED" : "❌ FAILED")}");
                
                Environment.Exit(success ? 0 : 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test execution failed: {ex.Message}");
                Environment.Exit(1);
            }
            finally
            {
                testClient.Dispose();
            }
        }
    }
}