# Simple Session Testing with Curl

If PowerShell has encoding issues, use these simple curl commands:

## Step 1: First Request (Cache Author)
```bash
curl -X POST "https://localhost:7207/api/Rag/ask" \
  -H "Content-Type: application/json" \
  -d "{\"question\": \"من هو نجيب محفوظ؟\", \"limit\": 5}" \
  -i
```

**Look for this in the output:**
- `X-Session-Id: <some-guid>` in the headers
- JSON response with author information

**Copy the Session ID from the headers!**

## Step 2: Second Request (Test Caching)
Replace `<SESSION_ID>` with the actual session ID from step 1:

```bash
curl -X POST "https://localhost:7207/api/Rag/ask" \
  -H "Content-Type: application/json" \
  -H "X-Session-Id: <SESSION_ID>" \
  -d "{\"question\": \"ما هي أشهر كتبه؟\", \"limit\": 5}"
```

## Success Criteria

### ✅ SUCCESS:
- Response contains book titles or information about Naguib Mahfouz's works
- NO message like "برجاء تحديد اسم المؤلف" (please specify author)

### ❌ FAILURE:
- Response asks to specify author name
- Generic response unrelated to books

## Alternative: Run Batch File
```cmd
test-session-simple.bat
```

This will automatically:
1. Make both requests
2. Extract session ID
3. Show you the results
4. Tell you if caching worked