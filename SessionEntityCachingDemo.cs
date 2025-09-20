// Session Entity Caching Demo for AseerAlkotb RAG System
// This demonstrates how the system caches entities (authors, books, publishers) 
// during conversation sessions and uses them in follow-up questions

using AseerAlkotb.Application.Features.Rag.Models;
using AseerAlkotb.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

// Example Usage Scenarios

Console.WriteLine("=== AseerAlkotb Session Entity Caching Demo ===\n");

// Scenario 1: User asks about an author, then asks for their books without mentioning the author name
Console.WriteLine("--- Scenario 1: Author Context Persistence ---");
Console.WriteLine("User: \"من هو نجيب محفوظ؟\" (Who is Naguib Mahfouz?)");
Console.WriteLine("System: Caches 'نجيب محفوظ' as the current author");
Console.WriteLine("User: \"ما هي أشهر كتبه؟\" (What are his most famous books?)");
Console.WriteLine("System: Uses cached author 'نجيب محفوظ' to find books by that author\n");

// Scenario 2: User asks about a book, then asks about availability without mentioning the book
Console.WriteLine("--- Scenario 2: Book Context Persistence ---");
Console.WriteLine("User: \"أريد ملخص كتاب الخيميائي\" (I want a summary of The Alchemist)");
Console.WriteLine("System: Caches 'الخيميائي' as the current book");
Console.WriteLine("User: \"هل هو متاح؟\" (Is it available?)");
Console.WriteLine("System: Uses cached book 'الخيميائي' to check availability\n");

// Scenario 3: User asks about a publisher, then asks for their books
Console.WriteLine("--- Scenario 3: Publisher Context Persistence ---");
Console.WriteLine("User: \"معلومات عن دار الشروق\" (Information about Dar Al-Shorouk)");
Console.WriteLine("System: Caches 'دار الشروق' as the current publisher");
Console.WriteLine("User: \"ما هي كتبهم المتاحة؟\" (What books do they have available?)");
Console.WriteLine("System: Uses cached publisher 'دار الشروق' to find their books\n");

// Technical Implementation Details
Console.WriteLine("--- Technical Implementation ---");
Console.WriteLine("✓ SessionMessage enhanced with EntityCache for resolved entity IDs");
Console.WriteLine("✓ SessionMemory provides methods to retrieve last mentioned entities");
Console.WriteLine("✓ SessionMemoryService caches normalized entity names and IDs");
Console.WriteLine("✓ RagService uses cached entities when router doesn't extract them");
Console.WriteLine("✓ Entity resolution includes: Books, Authors, Publishers, Categories");
Console.WriteLine("✓ Most recent entities take priority in multi-turn conversations\n");

// Key Benefits
Console.WriteLine("--- Key Benefits ---");
Console.WriteLine("🔹 Natural conversation flow - users don't need to repeat entity names");
Console.WriteLine("🔹 Context awareness - system remembers what was discussed");
Console.WriteLine("🔹 Improved user experience - more intuitive interactions");
Console.WriteLine("🔹 Reduced ambiguity - system uses conversation context to resolve intent");
Console.WriteLine("🔹 Session-based memory - temporary storage, no database persistence needed\n");

// Example API Usage
Console.WriteLine("--- API Usage Example ---");
Console.WriteLine("POST /api/rag/ask");
Console.WriteLine("Headers: { \"X-Session-Id\": \"user-session-123\" }");
Console.WriteLine("Body: { \"question\": \"من هو أحمد خالد توفيق؟\" }");
Console.WriteLine("Response: Cached author information");
Console.WriteLine();
Console.WriteLine("POST /api/rag/ask");
Console.WriteLine("Headers: { \"X-Session-Id\": \"user-session-123\" }");
Console.WriteLine("Body: { \"question\": \"ما هي أشهر كتبه؟\" }");
Console.WriteLine("Response: Uses cached author 'أحمد خالد توفيق' to find books\n");

Console.WriteLine("=== Entity Caching Implementation Complete ===");