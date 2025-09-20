# Test Session Entity Caching with curl

## Test Scenario: Ask about an author, then ask about their books

# Step 1: Ask about Naguib Mahfouz (this should cache the author)
curl -X POST 'https://localhost:7207/api/Rag/ask' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "question": "من هو نجيب محفوظ؟",
  "limit": 5
}'

# Note: Save the X-Session-Id from the response headers

# Step 2: Ask about his books using the same session ID (should use cached author)
curl -X POST 'https://localhost:7207/api/Rag/ask' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -H 'X-Session-Id: <SESSION_ID_FROM_STEP_1>' \
  -d '{
  "question": "ما هي أشهر كتبه؟",
  "limit": 5
}'

## Expected Results:
# Step 1: Should return author bio and cache "نجيب محفوظ"
# Step 2: Should automatically use cached author and return his books without asking "whose books?"

## Debugging:
# Check logs for messages like:
# "Session {SessionId}: Intent={Intent}, Author={Author}, Title={Title}, CachedAuthor={CachedAuthor}"
# "Detected 'more_by_author' intent from cached context for session {SessionId}"

## Alternative Test with PowerShell:

# Step 1:
$response1 = Invoke-RestMethod -Uri 'https://localhost:7207/api/Rag/ask' -Method POST -Headers @{'Content-Type'='application/json'} -Body '{"question":"من هو نجيب محفوظ؟","limit":5}'
$sessionId = $response1.Headers['X-Session-Id']

# Step 2:
$response2 = Invoke-RestMethod -Uri 'https://localhost:7207/api/Rag/ask' -Method POST -Headers @{'Content-Type'='application/json'; 'X-Session-Id'=$sessionId} -Body '{"question":"ما هي أشهر كتبه؟","limit":5}'