using System;
using System.Threading.Tasks;

namespace SimpleRagTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== اختبار نظام RAG ===");
            Console.WriteLine();

            // اختبار 1: إنشاء EmbeddingService
            Console.WriteLine("اختبار 1: إنشاء EmbeddingService");
            try
            {
                var embeddingService = new TestEmbeddingService();
                var embedding = await embeddingService.GenerateEmbeddingAsync("كتاب عن البرمجة");
                Console.WriteLine($"✅ نجح: تم إنشاء embedding بطول {embedding.Length}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ فشل: {ex.Message}");
            }

            // اختبار 2: إنشاء RagService
            Console.WriteLine("\nاختبار 2: إنشاء RagService");
            try
            {
                var ragService = new TestRagService();
                var response = await ragService.ProcessQueryAsync("أريد كتاب عن البرمجة");
                Console.WriteLine($"✅ نجح: {response.Answer}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ فشل: {ex.Message}");
            }

            // اختبار 3: إنشاء ExternalBookService
            Console.WriteLine("\nاختبار 3: إنشاء ExternalBookService");
            try
            {
                var externalService = new TestExternalBookService();
                var summary = await externalService.GetBookSummaryAsync("Clean Code", "Robert Martin");
                Console.WriteLine($"✅ نجح: {summary.Substring(0, Math.Min(100, summary.Length))}...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ فشل: {ex.Message}");
            }

            Console.WriteLine("\n=== انتهى الاختبار ===");
        }
    }

    // Mock classes for testing
    public class TestEmbeddingService
    {
        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            await Task.Delay(100); // Simulate async work
            var embedding = new float[100];
            var random = new Random(text.GetHashCode());
            for (int i = 0; i < embedding.Length; i++)
            {
                embedding[i] = (float)random.NextDouble();
            }
            return embedding;
        }
    }

    public class TestRagService
    {
        public async Task<TestRagResponse> ProcessQueryAsync(string query)
        {
            await Task.Delay(100); // Simulate async work
            return new TestRagResponse
            {
                Answer = "تم معالجة الاستعلام بنجاح: " + query,
                Confidence = 0.8
            };
        }
    }

    public class TestExternalBookService
    {
        public async Task<string> GetBookSummaryAsync(string bookTitle, string authorName)
        {
            await Task.Delay(100); // Simulate async work
            return $"ملخص كتاب {bookTitle} للكاتب {authorName}: هذا كتاب ممتاز في البرمجة...";
        }
    }

    public class TestRagResponse
    {
        public string Answer { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }
}
