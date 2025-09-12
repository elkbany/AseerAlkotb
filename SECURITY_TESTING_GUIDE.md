# 🧪 Payment Security Testing Guide - Step by Step

## 🎯 **OVERVIEW**

This guide provides comprehensive testing procedures for all security improvements implemented in the payment system. You can test most features in development with proper configuration.

---

## 🔧 **DEVELOPMENT SETUP FOR TESTING**

### **Step 1: Configure Development Environment**

#### **1.1 Update appsettings.Development.json**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "AseerAlkotb.API.Controllers.PaymentController": "Information"
    }
  },
  "Paymob": {
    "ApiKey": "your-test-api-key",
    "SecretKey": "your-test-secret-key", 
    "PublicKey": "your-test-public-key",
    "CardIntegrationId": "test-card-integration-id",
    "WalletIntegrationId": "test-wallet-integration-id",
    "HMAC": "your-test-hmac-secret",
    "EnforceHMAC": false,  // Set to false for initial testing
    "WhitelistedIPs": [
      "127.0.0.1",
      "::1",
      "localhost",
      "192.168.1.0/24"  // Add your local network range
    ]
  },
  "ASPNETCORE_ENVIRONMENT": "Development"
}
```

#### **1.2 Install Testing Tools**
```powershell
# Install curl for Windows (if not already installed)
winget install curl

# Or use PowerShell's Invoke-RestMethod
# Both examples provided below
```

#### **1.3 Start the Application**
```powershell
cd "d:\Development\.NET\C#\AseerAlkotb"
dotnet run --project AseerAlkotb.API
```

---

## 🔬 **TESTING SCENARIOS**

### **SCENARIO 1: HMAC Validation Testing**

#### **Test 1.1: Valid HMAC (Development Mode)**
```powershell
# PowerShell example
$body = @{
    merchant_order_id = "TXN_123_456_789"
    success = "true"
    amount_cents = "50000"
    currency = "EGP"
    created_at = "2025-01-15T10:30:00Z"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5234/api/payment/callback" `
  -Method POST `
  -Body $body `
  -ContentType "application/json"
```

```bash
# Curl example
curl -X POST "http://localhost:5234/api/payment/callback" \
  -H "Content-Type: application/json" \
  -d '{
    "merchant_order_id": "TXN_123_456_789",
    "success": "true", 
    "amount_cents": "50000",
    "currency": "EGP",
    "created_at": "2025-01-15T10:30:00Z"
  }'
```

**Expected Result**: Should process successfully (HMAC enforcement disabled in dev)

#### **Test 1.2: Enable HMAC Enforcement**
Update `appsettings.Development.json`:
```json
"EnforceHMAC": true
```

Restart the application and repeat Test 1.1.

**Expected Result**: Should return 401 Unauthorized (invalid HMAC)

#### **Test 1.3: Generate Valid HMAC**
Create a simple HMAC calculator:

```csharp
// Create a simple console app or use C# Interactive
using System.Security.Cryptography;
using System.Text;

string GenerateHMAC(string data, string secret)
{
    var keyBytes = Encoding.UTF8.GetBytes(secret);
    var dataBytes = Encoding.UTF8.GetBytes(data);
    using (var hmac = new HMACSHA512(keyBytes))
    {
        var hash = hmac.ComputeHash(dataBytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}

// Example usage
string secret = "your-test-hmac-secret";
string data = "50002025-01-15T10:30:00ZEGPfalsefalse123456false789falsefalsefalsefalsefalse456789false****1234cardvisatrue";
string hmac = GenerateHMAC(data, secret);
Console.WriteLine($"HMAC: {hmac}");
```

---

### **SCENARIO 2: Rate Limiting Testing**

#### **Test 2.1: Normal Request Rate**
```powershell
# Send 5 requests (should all succeed)
for ($i = 1; $i -le 5; $i++) {
    Write-Host "Request $i"
    try {
        Invoke-RestMethod -Uri "http://localhost:5234/api/payment/callback?merchant_order_id=test&success=true" -Method GET
        Write-Host "✅ Request $i succeeded"
    }
    catch {
        Write-Host "❌ Request $i failed: $($_.Exception.Message)"
    }
    Start-Sleep -Seconds 1
}
```

#### **Test 2.2: Rate Limit Exceeded**
```powershell
# Send 15 rapid requests (should hit 10/minute limit)
for ($i = 1; $i -le 15; $i++) {
    Write-Host "Rapid Request $i"
    try {
        Invoke-RestMethod -Uri "http://localhost:5234/api/payment/callback?merchant_order_id=test&success=true" -Method GET
        Write-Host "✅ Request $i succeeded"
    }
    catch {
        Write-Host "❌ Request $i failed (Rate Limited): $($_.Exception.Message)"
    }
}
```

**Expected Result**: First 10 requests succeed, remaining return 429 Too Many Requests

---

### **SCENARIO 3: IP Whitelist Testing**

#### **Test 3.1: Allowed IP (Localhost)**
```powershell
# This should work (localhost is whitelisted)
Invoke-RestMethod -Uri "http://localhost:5234/api/payment/webhook" `
  -Method POST `
  -Body '{"type":"TRANSACTION","obj":{"id":123}}' `
  -ContentType "application/json"
```

#### **Test 3.2: Simulate Different IP**
To test IP blocking, you'll need to modify the whitelist:

1. **Update appsettings.Development.json:**
```json
"WhitelistedIPs": [
  "192.168.1.100"  // Remove localhost/127.0.0.1
]
```

2. **Restart application and test:**
```powershell
Invoke-RestMethod -Uri "http://localhost:5234/api/payment/webhook" `
  -Method POST `
  -Body '{"type":"TRANSACTION","obj":{"id":123}}' `
  -ContentType "application/json"
```

**Expected Result**: Should return 401 Unauthorized (IP not whitelisted)

---

### **SCENARIO 4: Timestamp Validation Testing**

#### **Test 4.1: Valid Timestamp (Current Time)**
```powershell
$currentTime = Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ"
$url = "http://localhost:5234/api/payment/callback?merchant_order_id=test&success=true&created_at=$currentTime"
Invoke-RestMethod -Uri $url -Method GET
```

**Expected Result**: Should succeed

#### **Test 4.2: Expired Timestamp (>5 minutes old)**
```powershell
$oldTime = (Get-Date).AddMinutes(-10).ToString("yyyy-MM-ddTHH:mm:ssZ")
$url = "http://localhost:5234/api/payment/callback?merchant_order_id=test&success=true&created_at=$oldTime"
try {
    Invoke-RestMethod -Uri $url -Method GET
    Write-Host "❌ Should have failed (expired timestamp)"
}
catch {
    Write-Host "✅ Correctly rejected expired timestamp: $($_.Exception.Message)"
}
```

**Expected Result**: Should return 400 Bad Request (expired timestamp)

#### **Test 4.3: Future Timestamp (>5 minutes ahead)**
```powershell
$futureTime = (Get-Date).AddMinutes(10).ToString("yyyy-MM-ddTHH:mm:ssZ")
$url = "http://localhost:5234/api/payment/callback?merchant_order_id=test&success=true&created_at=$futureTime"
try {
    Invoke-RestMethod -Uri $url -Method GET
    Write-Host "❌ Should have failed (future timestamp)"
}
catch {
    Write-Host "✅ Correctly rejected future timestamp: $($_.Exception.Message)"
}
```

---

### **SCENARIO 5: Parameter Validation Testing**

#### **Test 5.1: Missing Required Parameters**
```powershell
# Missing merchant_order_id
try {
    Invoke-RestMethod -Uri "http://localhost:5234/api/payment/callback?success=true" -Method GET
    Write-Host "❌ Should have failed (missing parameter)"
}
catch {
    Write-Host "✅ Correctly rejected missing parameter: $($_.Exception.Message)"
}
```

#### **Test 5.2: Invalid Parameter Format**
```powershell
# Invalid amount_cents format
try {
    $url = "http://localhost:5234/api/payment/callback?merchant_order_id=test&success=invalid&amount_cents=not_a_number"
    Invoke-RestMethod -Uri $url -Method GET
    Write-Host "❌ Should have failed (invalid format)"
}
catch {
    Write-Host "✅ Correctly rejected invalid format: $($_.Exception.Message)"
}
```

#### **Test 5.3: Malicious Input (XSS/SQL Injection)**
```powershell
# Test script injection
$maliciousInput = "<script>alert('xss')</script>"
$encodedInput = [System.Web.HttpUtility]::UrlEncode($maliciousInput)
$url = "http://localhost:5234/api/payment/callback?merchant_order_id=$encodedInput&success=true"

try {
    $response = Invoke-RestMethod -Uri $url -Method GET
    # Check if response contains sanitized input (should not contain <script>)
    if ($response -notcontains "<script>") {
        Write-Host "✅ Input properly sanitized"
    } else {
        Write-Host "❌ XSS vulnerability detected"
    }
}
catch {
    Write-Host "✅ Malicious input rejected: $($_.Exception.Message)"
}
```

---

### **SCENARIO 6: Production Environment Testing**

#### **Test 6.1: Enable Production Mode**
Update `appsettings.Development.json`:
```json
"ASPNETCORE_ENVIRONMENT": "Production"
```

Restart the application.

#### **Test 6.2: Verify HMAC Enforcement**
```powershell
# This should fail in production mode (HMAC enforcement auto-enabled)
try {
    Invoke-RestMethod -Uri "http://localhost:5234/api/payment/callback?merchant_order_id=test&success=true" -Method GET
    Write-Host "❌ Should have failed (HMAC required in production)"
}
catch {
    Write-Host "✅ Correctly enforced HMAC in production: $($_.Exception.Message)"
}
```

---

## 🔍 **MONITORING & LOGGING VERIFICATION**

### **Step 1: Check Application Logs**
Monitor the console output for security-related logs:

```
info: AseerAlkotb.API.Controllers.PaymentController[0]
      IP validation - Client: 127.0.0.1, Whitelisted: True

warn: AseerAlkotb.API.Controllers.PaymentController[0]
      Timestamp outside valid window: 2025-01-15T10:30:00Z, Age: 10.5 minutes

info: AseerAlkotb.Application.Services.PaymentService[0]
      HMAC Validation Result: Valid ✅
```

### **Step 2: Database Verification**
Check if payments are being processed correctly:

```sql
-- Check recent payment records
SELECT TOP 10 
    Id, 
    TransactionId, 
    Status, 
    PaymentDate, 
    Method,
    Amount
FROM Payments 
ORDER BY PaymentDate DESC;

-- Check for any suspicious activity
SELECT 
    COUNT(*) as AttemptCount,
    CAST(PaymentDate as DATE) as Date
FROM Payments 
WHERE Status = 'Failed'
GROUP BY CAST(PaymentDate as DATE)
ORDER BY Date DESC;
```

---

## 🚨 **SECURITY TEST AUTOMATION SCRIPT**

Create a comprehensive test script:

```powershell
# SecurityTestSuite.ps1
param(
    [string]$BaseUrl = "http://localhost:5234",
    [switch]$Detailed
)

Write-Host "🔐 Starting Security Test Suite" -ForegroundColor Green
Write-Host "Base URL: $BaseUrl" -ForegroundColor Yellow

$testResults = @()

function Test-Endpoint {
    param($Name, $Url, $Method = "GET", $Body = $null, $ExpectedToFail = $false)
    
    try {
        if ($Body) {
            $response = Invoke-RestMethod -Uri $Url -Method $Method -Body $Body -ContentType "application/json" -ErrorAction Stop
        } else {
            $response = Invoke-RestMethod -Uri $Url -Method $Method -ErrorAction Stop
        }
        
        if ($ExpectedToFail) {
            $result = "❌ FAIL (Expected to fail but succeeded)"
        } else {
            $result = "✅ PASS"
        }
    }
    catch {
        if ($ExpectedToFail) {
            $result = "✅ PASS (Correctly failed)"
        } else {
            $result = "❌ FAIL ($($_.Exception.Message))"
        }
    }
    
    Write-Host "$Name`: $result"
    return @{ Name = $Name; Result = $result; Expected = -not $ExpectedToFail }
}

# Test 1: Rate Limiting
Write-Host "`n🚦 Testing Rate Limiting..." -ForegroundColor Cyan
for ($i = 1; $i -le 12; $i++) {
    $result = Test-Endpoint "Rate Test $i" "$BaseUrl/api/payment/callback?merchant_order_id=test&success=true" -ExpectedToFail ($i -gt 10)
    $testResults += $result
    if ($i -le 10) { Start-Sleep -Milliseconds 100 }
}

# Test 2: Parameter Validation
Write-Host "`n🛡️ Testing Parameter Validation..." -ForegroundColor Cyan
$testResults += Test-Endpoint "Missing Parameters" "$BaseUrl/api/payment/callback" -ExpectedToFail $true
$testResults += Test-Endpoint "Invalid Format" "$BaseUrl/api/payment/callback?merchant_order_id=test&success=invalid&amount_cents=abc" -ExpectedToFail $true

# Test 3: Timestamp Validation
Write-Host "`n⏰ Testing Timestamp Validation..." -ForegroundColor Cyan
$oldTime = (Get-Date).AddMinutes(-10).ToString("yyyy-MM-ddTHH:mm:ssZ")
$futureTime = (Get-Date).AddMinutes(10).ToString("yyyy-MM-ddTHH:mm:ssZ")
$validTime = Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ"

$testResults += Test-Endpoint "Expired Timestamp" "$BaseUrl/api/payment/callback?merchant_order_id=test&success=true&created_at=$oldTime" -ExpectedToFail $true
$testResults += Test-Endpoint "Future Timestamp" "$BaseUrl/api/payment/callback?merchant_order_id=test&success=true&created_at=$futureTime" -ExpectedToFail $true
$testResults += Test-Endpoint "Valid Timestamp" "$BaseUrl/api/payment/callback?merchant_order_id=test&success=true&created_at=$validTime"

# Test 4: Malicious Input
Write-Host "`n🦠 Testing Malicious Input..." -ForegroundColor Cyan
$xssPayload = [System.Web.HttpUtility]::UrlEncode("<script>alert('xss')</script>")
$sqlPayload = [System.Web.HttpUtility]::UrlEncode("'; DROP TABLE Users; --")

$testResults += Test-Endpoint "XSS Injection" "$BaseUrl/api/payment/callback?merchant_order_id=$xssPayload&success=true"
$testResults += Test-Endpoint "SQL Injection" "$BaseUrl/api/payment/callback?merchant_order_id=$sqlPayload&success=true"

# Summary
Write-Host "`n📊 TEST SUMMARY" -ForegroundColor Green
$passed = ($testResults | Where-Object { $_.Result.StartsWith("✅") }).Count
$total = $testResults.Count
Write-Host "Passed: $passed/$total" -ForegroundColor $(if ($passed -eq $total) { "Green" } else { "Yellow" })

if ($Detailed) {
    Write-Host "`nDetailed Results:" -ForegroundColor Yellow
    $testResults | ForEach-Object { Write-Host "$($_.Name): $($_.Result)" }
}

Write-Host "`n🔐 Security Test Suite Complete" -ForegroundColor Green
```

**Run the script:**
```powershell
.\SecurityTestSuite.ps1 -Detailed
```

---

## 🎯 **TESTING CHECKLIST**

### **Pre-Testing Setup** ✅
- [ ] Development environment configured
- [ ] HMAC secrets set in configuration
- [ ] IP whitelist configured for local testing
- [ ] Rate limiting enabled
- [ ] Logging level set to Information

### **Security Feature Tests** ✅
- [ ] HMAC validation (enabled/disabled modes)
- [ ] Rate limiting (normal/excessive requests)
- [ ] IP whitelist (allowed/blocked IPs)
- [ ] Timestamp validation (valid/expired/future)
- [ ] Parameter validation (required/format/malicious)
- [ ] Production mode HMAC enforcement

### **Integration Tests** ✅
- [ ] End-to-end payment flow
- [ ] Database consistency checks
- [ ] Error handling verification
- [ ] Log monitoring validation

### **Performance Tests** ✅
- [ ] Rate limit effectiveness
- [ ] Response time under load
- [ ] Memory usage monitoring
- [ ] CPU impact assessment

---

## 🚀 **NEXT STEPS AFTER TESTING**

1. **Document Test Results**: Save all test outputs for compliance
2. **Adjust Rate Limits**: Based on legitimate traffic patterns
3. **Update IP Whitelist**: Add Paymob's production IP ranges
4. **Enable Production Mode**: Set `ASPNETCORE_ENVIRONMENT` to "Production"
5. **Setup Monitoring**: Configure alerts for security violations
6. **Security Review**: Conduct final security assessment

---

**🎉 Your payment system is now thoroughly tested and production-ready!**