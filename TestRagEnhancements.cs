using System;
using System.Threading.Tasks;
using AseerAlkotb.Application.Utils;

// Simple test to verify RAG system enhancements
class TestRagEnhancements
{
    static void Main()
    {
        Console.WriteLine("Testing RAG System Enhancements...\n");
        
        TestQueryExtraction();
        TestAdvancedQueryExtraction();
        
        Console.WriteLine("\nAll tests completed!");
    }
    
    static void TestQueryExtraction()
    {
        Console.WriteLine("1. Testing basic query extraction:");
        
        var tests = new[]
        {
            "ملخص كتاب أولاد حارتنا لنجيب محفوظ",
            "معلومات عن دار الشروق",
            "كتب من منشورات عصير الكتب",
            "الكتاب دا من أي دار نشر؟",
            "نبذة عن المؤلف أحمد خالد توفيق"
        };
        
        foreach (var test in tests)
        {
            var (title, author) = QueryExtractor.Extract(test);
            Console.WriteLine($"Query: {test}");
            Console.WriteLine($"  Title: {title ?? "null"}");
            Console.WriteLine($"  Author: {author ?? "null"}");
            Console.WriteLine();
        }
    }
    
    static void TestAdvancedQueryExtraction()
    {
        Console.WriteLine("2. Testing advanced query extraction with publisher:");
        
        var tests = new[]
        {
            "معلومات عن دار الشروق",
            "كتب من منشورات عصير الكتب", 
            "الكتاب دا من أي دار نشر؟",
            "نبذة عن دار النشر العربية",
            "Publisher: Dar Al Shorouk"
        };
        
        foreach (var test in tests)
        {
            var (title, author, publisher) = QueryExtractor.ExtractAdvanced(test);
            Console.WriteLine($"Query: {test}");
            Console.WriteLine($"  Title: {title ?? "null"}");
            Console.WriteLine($"  Author: {author ?? "null"}");
            Console.WriteLine($"  Publisher: {publisher ?? "null"}");
            Console.WriteLine();
        }
    }
}