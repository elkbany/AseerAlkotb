# Manual Session Caching Test Checklist

## Prerequisites
- [ ] AseerAlkotb.API is running (usually on https://localhost:7207)
- [ ] Swagger UI is accessible
- [ ] You have a REST client (Postman, curl, or browser developer tools)

## Test Method 1: Using PowerShell Script (Easiest)

1. **Run the PowerShell script:**
   ```powershell
   cd "c:\Users\Hi-Tech\OneDrive\Desktop\NewFinalProject\AseerAlkotb"
   .\test-session-caching.ps1
   ```

2. **Expected Output:**
   ```
   ✅ Session ID received: <guid>
   ✅ Response: <author bio>
   ✅ SUCCESS: System used cached author context!
   ✅ Response: <list of books>
   ✅ Session ID persistence: PASSED
   ✅ Entity caching: PASSED
   ```

## Test Method 2: Using Swagger UI (Visual)

### Step 1: First Request (Cache Author)
1. Open Swagger UI: `https://localhost:7207/swagger`
2. Navigate to `/api/Rag/ask`
3. Click "Try it out"
4. Enter request body:
   ```json
   {
     "question": "من هو نجيب محفوظ؟",
     "limit": 5
   }
   ```
5. Click "Execute"
6. **COPY the `X-Session-Id` from response headers**
7. Verify response contains author information

### Step 2: Second Request (Test Cached Context)
1. **Before executing:** Add header parameter:
   - Name: `X-Session-Id`
   - Value: `<session-id-from-step-1>`
2. Enter request body:
   ```json
   {
     "question": "ما هي أشهر كتبه؟",
     "limit": 5
   }
   ```
3. Click "Execute"

### Success Criteria:
- [ ] ✅ Response lists author's books
- [ ] ✅ NO generic error like "برجاء تحديد اسم المؤلف"
- [ ] ✅ Session ID remains consistent

## Test Method 3: Using curl (Command Line)

### First Request:
```bash
curl -X POST "https://localhost:7207/api/Rag/ask" \
  -H "accept: text/plain" \
  -H "Content-Type: application/json" \
  -d '{
    "question": "من هو نجيب محفوظ؟",
    "limit": 5
  }' \
  -v
```

### Extract Session ID from response headers, then:
```bash
curl -X POST "https://localhost:7207/api/Rag/ask" \
  -H "accept: text/plain" \
  -H "Content-Type: application/json" \
  -H "X-Session-Id: <SESSION_ID_FROM_FIRST_REQUEST>" \
  -d '{
    "question": "ما هي أشهر كتبه؟",
    "limit": 5
  }'
```

## Troubleshooting

### ❌ Problem: "برجاء تحديد اسم المؤلف" in second response
**Solutions:**
- [ ] Verify session ID is correctly passed in header
- [ ] Check that header name is exactly `X-Session-Id`
- [ ] Ensure API server has the latest code with session caching

### ❌ Problem: No session ID in response headers
**Solutions:**
- [ ] Restart the API server
- [ ] Check if RagController has the updated code
- [ ] Verify the API is running the correct version

### ❌ Problem: Session ID changes between requests
**Solutions:**
- [ ] Check that you're passing the session ID correctly
- [ ] Verify header spelling and case sensitivity
- [ ] Ensure no extra characters in session ID

## Additional Test Scenarios

### Book Context Test:
1. First: `"أريد ملخص كتاب الخيميائي"`
2. Second: `"هل هو متاح؟"` (should check availability of "الخيميائي")

### Publisher Context Test:
1. First: `"معلومات عن دار الشروق"`
2. Second: `"ما هي كتبهم المتاحة؟"` (should list books from "دار الشروق")

## Expected Behavior Summary

| Test Step | Expected Result |
|-----------|----------------|
| First Request | Returns author bio + session ID |
| Second Request | Uses cached author, returns books list |
| Session ID | Consistent across requests |
| Context Usage | No "please specify" errors |

## Logging (Optional)
Check application console logs for:
- `"Session {SessionId}: Intent={Intent}, Author={Author}..."`
- `"Detected 'more_by_author' intent from cached context"`

## Success Indicators
- [ ] ✅ Session ID is generated and returned
- [ ] ✅ Session ID persists across requests
- [ ] ✅ Follow-up questions use cached context
- [ ] ✅ No entity specification errors
- [ ] ✅ Responses are contextually relevant