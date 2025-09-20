# Check the test responses
Write-Host "=== Session Test Results Analysis ===" -ForegroundColor Cyan
Write-Host ""

# Check if response files exist
if (Test-Path "response1.json") {
    Write-Host "📄 First Response (Author Info):" -ForegroundColor Yellow
    $response1 = Get-Content "response1.json" | ConvertFrom-Json
    Write-Host "✅ Answer: $($response1.data.answer)" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host "❌ response1.json not found" -ForegroundColor Red
}

if (Test-Path "response2.json") {
    Write-Host "📄 Second Response (Books Query):" -ForegroundColor Yellow
    $response2 = Get-Content "response2.json" | ConvertFrom-Json
    Write-Host "✅ Answer: $($response2.data.answer)" -ForegroundColor Green
    Write-Host ""
    
    # Analyze if caching worked
    Write-Host "=== Analysis ===" -ForegroundColor Cyan
    $answer = $response2.data.answer
    
    if ($answer -match "برجاء تحديد" -or $answer -match "تحديد اسم المؤلف") {
        Write-Host "❌ CACHING FAILED: System asked to specify author" -ForegroundColor Red
    } elseif ($answer -match "كتب" -or $answer -match "مؤلفات" -or $answer -match "نجيب") {
        Write-Host "✅ CACHING SUCCESS: System used cached author context!" -ForegroundColor Green
    } else {
        Write-Host "⚠️  UNCLEAR: Cannot determine if caching worked from response" -ForegroundColor Yellow
    }
} else {
    Write-Host "❌ response2.json not found" -ForegroundColor Red
}

Write-Host ""
Write-Host "📁 You can also manually check:" -ForegroundColor Gray
Write-Host "   - response1.json (first response)" -ForegroundColor Gray
Write-Host "   - response2.json (second response)" -ForegroundColor Gray