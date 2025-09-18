// Test to verify JSON parsing handles array intent values
using System.Text.Json;
using AseerAlkotb.Application.Features.Rag.Models;

namespace AseerAlkotb.Tests
{
    public class JsonParsingTest
    {
        public static void TestArrayIntentHandling()
        {
            // Test cases that might return arrays instead of strings
            var testJsons = new[]
            {
                // Normal case - string intent
                @"{""intent"": ""price"", ""entities"": {""title"": ""بين القصرين""}, ""language"": ""ar"", ""confidence"": 0.9}",
                
                // Problematic case - array intent (what was causing the error)
                @"{""intent"": [""price"", ""availability""], ""entities"": {""title"": ""بين القصرين""}, ""language"": ""ar"", ""confidence"": 0.8}",
                
                // Edge case - empty array
                @"{""intent"": [], ""entities"": {""title"": ""بين القصرين""}, ""language"": ""ar"", ""confidence"": 0.5}",
                
                // Edge case - null intent
                @"{""intent"": null, ""entities"": {""title"": ""بين القصرين""}, ""language"": ""ar"", ""confidence"": 0.3}"
            };

            Console.WriteLine("Testing JSON parsing with different intent formats:");
            Console.WriteLine("===============================================");

            foreach (var json in testJsons)
            {
                try
                {
                    using var jsonDoc = JsonDocument.Parse(json);
                    var root = jsonDoc.RootElement;
                    
                    // Handle intent - could be string or array
                    string? intentValue = null;
                    if (root.TryGetProperty("intent", out var intentProp))
                    {
                        if (intentProp.ValueKind == JsonValueKind.String)
                        {
                            intentValue = intentProp.GetString();
                        }
                        else if (intentProp.ValueKind == JsonValueKind.Array && intentProp.GetArrayLength() > 0)
                        {
                            // Take the first intent if it's an array
                            intentValue = intentProp[0].GetString();
                        }
                    }
                    
                    // Extract entities
                    var entities = new RouteEntities();
                    if (root.TryGetProperty("entities", out var entitiesProp))
                    {
                        if (entitiesProp.TryGetProperty("title", out var titleProp))
                            entities.title = titleProp.ValueKind == JsonValueKind.String ? titleProp.GetString() : null;
                    }
                    
                    // Extract other properties
                    var language = "ar";
                    if (root.TryGetProperty("language", out var langProp) && langProp.ValueKind == JsonValueKind.String)
                        language = langProp.GetString() ?? "ar";
                        
                    var confidence = 0.0;
                    if (root.TryGetProperty("confidence", out var confProp) && confProp.ValueKind == JsonValueKind.Number)
                        confidence = confProp.GetDouble();
                    
                    var result = new RouteResult
                    {
                        intent = intentValue ?? "general_recs",
                        entities = entities,
                        language = language,
                        confidence = confidence
                    };
                    
                    Console.WriteLine($"✅ Parsed successfully:");
                    Console.WriteLine($"   Input: {json}");
                    Console.WriteLine($"   Intent: {result.intent}");
                    Console.WriteLine($"   Title: {result.entities.title}");
                    Console.WriteLine($"   Confidence: {result.confidence:F2}");
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error parsing: {json}");
                    Console.WriteLine($"   Error: {ex.Message}");
                    Console.WriteLine();
                }
            }
        }
    }
}