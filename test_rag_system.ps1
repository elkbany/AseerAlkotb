# اختبار نظام RAG
Write-Host "بدء اختبار نظام RAG..." -ForegroundColor Green

# اختبار 1: معالجة استعلام بسيط
Write-Host "`nاختبار 1: معالجة استعلام بسيط" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5234/api/rag/ask" -Method POST -Body '{"query": "أريد كتاب عن البرمجة"}' -ContentType "application/json"
    Write-Host "✅ نجح الاختبار: $($response.data.answer)" -ForegroundColor Green
} catch {
    Write-Host "❌ فشل الاختبار: $($_.Exception.Message)" -ForegroundColor Red
}

# اختبار 2: جلب ملخص كتاب من الإنترنت
Write-Host "`nاختبار 2: جلب ملخص كتاب من الإنترنت" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5234/api/rag/book-summary?bookTitle=Clean Code&authorName=Robert Martin" -Method GET
    Write-Host "✅ نجح الاختبار: $($response.data.Substring(0, 100))..." -ForegroundColor Green
} catch {
    Write-Host "❌ فشل الاختبار: $($_.Exception.Message)" -ForegroundColor Red
}

# اختبار 3: البحث الذكي
Write-Host "`nاختبار 3: البحث الذكي" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5234/api/rag/smart-search?searchQuery=برمجة" -Method GET
    Write-Host "✅ نجح الاختبار: تم العثور على $($response.data.Count) نتيجة" -ForegroundColor Green
} catch {
    Write-Host "❌ فشل الاختبار: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nانتهى اختبار نظام RAG" -ForegroundColor Green
