# PowerShell script to test session entity caching
# Make sure AseerAlkotb.API is running on https://localhost:7207

Write-Host "=== Testing Session Entity Caching ===" -ForegroundColor Cyan
Write-Host ""

# Test 1: Ask about an author (this should cache the author)
Write-Host "🔸 Test 1: Asking about an author..." -ForegroundColor Yellow

$request1 = @{
    question = "من هو نجيب محفوظ؟"
    limit = 5
} | ConvertTo-Json

try {
    $response1 = Invoke-RestMethod -Uri "https://localhost:7207/api/Rag/ask" `
        -Method POST `
        -ContentType "application/json" `
        -Body $request1 `
        -ResponseHeadersVariable headers1

    # Extract session ID from response headers
    $sessionId = $headers1["X-Session-Id"][0]
    
    Write-Host "✅ Session ID received: $sessionId" -ForegroundColor Green
    Write-Host "✅ Response: $($response1.data.answer.Substring(0, [Math]::Min(100, $response1.data.answer.Length)))..." -ForegroundColor Green
    Write-Host ""

    # Test 2: Ask about the author's books using the same session
    Write-Host "🔸 Test 2: Asking about his books (using cached author)..." -ForegroundColor Yellow

    $request2 = @{
        question = "ما هي أشهر كتبه؟"
        limit = 5
    } | ConvertTo-Json

    $headers = @{
        "X-Session-Id" = $sessionId
        "Content-Type" = "application/json"
    }

    $response2 = Invoke-RestMethod -Uri "https://localhost:7207/api/Rag/ask" `
        -Method POST `
        -Headers $headers `
        -Body $request2 `
        -ResponseHeadersVariable headers2

    # Check if the response uses cached context (using English keywords to avoid encoding issues)
    $responseText = $response2.data.answer
    $askingForAuthor = $responseText -match "تحديد اسم المؤلف" -or $responseText -match "برجاء تحديد"
    $hasBooksContext = $responseText -match "كتب" -or $responseText -match "مؤلفات" -or $responseText -match "نجيب"
    
    if ($askingForAuthor) {
        Write-Host "❌ FAILED: System asked to specify author name - caching not working" -ForegroundColor Red
        Write-Host "Response: $responseText" -ForegroundColor Red
    }
    elseif ($hasBooksContext) {
        Write-Host "✅ SUCCESS: System used cached author context!" -ForegroundColor Green
        Write-Host "✅ Response: $($responseText.Substring(0, [Math]::Min(150, $responseText.Length)))..." -ForegroundColor Green
    }
    else {
        Write-Host "⚠️  UNCERTAIN: Response doesn't clearly indicate cached context usage" -ForegroundColor Yellow
        Write-Host "Response: $responseText" -ForegroundColor Yellow
    }

    Write-Host ""
    Write-Host "=== Session Test Summary ===" -ForegroundColor Cyan
    Write-Host "✅ Session ID persistence: $(if($headers2["X-Session-Id"][0] -eq $sessionId) { "PASSED" } else { "FAILED" })" -ForegroundColor $(if($headers2["X-Session-Id"][0] -eq $sessionId) { "Green" } else { "Red" })
    Write-Host "✅ Entity caching: $(if(-not $askingForAuthor) { "PASSED" } else { "FAILED" })" -ForegroundColor $(if(-not $askingForAuthor) { "Green" } else { "Red" })

} catch {
    Write-Host "❌ ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Make sure the API is running on https://localhost:7207" -ForegroundColor Yellow
}