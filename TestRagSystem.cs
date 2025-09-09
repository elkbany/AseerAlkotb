using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestRagSystem
{
    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string BaseUrl = "http://localhost:5234/api/rag";

        static async Task Main(string[] args)
        {
            Console.WriteLine("بدء اختبار نظام RAG...");
            Console.WriteLine("=====================================");

            // اختبار 1: معالجة استعلام بسيط
            await TestAskQuestion();

            // اختبار 2: جلب ملخص كتاب من الإنترنت
            await TestBookSummary();

            // اختبار 3: البحث الذكي
            await TestSmartSearch();

            // اختبار 4: جلب كتب الكاتب
            await TestAuthorBooks();

            // اختبار 5: جلب كتب الفئة
            await TestCategoryBooks();

            Console.WriteLine("\nانتهى اختبار نظام RAG");
        }

        static async Task TestAskQuestion()
        {
            Console.WriteLine("\nاختبار 1: معالجة استعلام بسيط");
            try
            {
                var request = new { query = "أريد كتاب عن البرمجة" };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{BaseUrl}/ask", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ نجح الاختبار: تم معالجة الاستعلام بنجاح");
                    Console.WriteLine($"الاستجابة: {responseContent.Substring(0, Math.Min(100, responseContent.Length))}...");
                }
                else
                {
                    Console.WriteLine($"❌ فشل الاختبار: {response.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ فشل الاختبار: {ex.Message}");
            }
        }

        static async Task TestBookSummary()
        {
            Console.WriteLine("\nاختبار 2: جلب ملخص كتاب من الإنترنت");
            try
            {
                var response = await httpClient.GetAsync($"{BaseUrl}/book-summary?bookTitle=Clean Code&authorName=Robert Martin");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ نجح الاختبار: تم جلب الملخص بنجاح");
                    Console.WriteLine($"الملخص: {responseContent.Substring(0, Math.Min(100, responseContent.Length))}...");
                }
                else
                {
                    Console.WriteLine($"❌ فشل الاختبار: {response.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ فشل الاختبار: {ex.Message}");
            }
        }

        static async Task TestSmartSearch()
        {
            Console.WriteLine("\nاختبار 3: البحث الذكي");
            try
            {
                var response = await httpClient.GetAsync($"{BaseUrl}/smart-search?searchQuery=برمجة");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ نجح الاختبار: تم البحث بنجاح");
                    Console.WriteLine($"النتائج: {responseContent}");
                }
                else
                {
                    Console.WriteLine($"❌ فشل الاختبار: {response.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ فشل الاختبار: {ex.Message}");
            }
        }

        static async Task TestAuthorBooks()
        {
            Console.WriteLine("\nاختبار 4: جلب كتب الكاتب");
            try
            {
                var response = await httpClient.GetAsync($"{BaseUrl}/author-books/أحمد خالد توفيق");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ نجح الاختبار: تم جلب كتب الكاتب بنجاح");
                    Console.WriteLine($"النتائج: {responseContent}");
                }
                else
                {
                    Console.WriteLine($"❌ فشل الاختبار: {response.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ فشل الاختبار: {ex.Message}");
            }
        }

        static async Task TestCategoryBooks()
        {
            Console.WriteLine("\nاختبار 5: جلب كتب الفئة");
            try
            {
                var response = await httpClient.GetAsync($"{BaseUrl}/category-books/البرمجة");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ نجح الاختبار: تم جلب كتب الفئة بنجاح");
                    Console.WriteLine($"النتائج: {responseContent}");
                }
                else
                {
                    Console.WriteLine($"❌ فشل الاختبار: {response.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ فشل الاختبار: {ex.Message}");
            }
        }
    }
}
