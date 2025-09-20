# Testing Session Entity Caching with Swagger UI

## Overview
This guide shows how to test the session entity caching feature using Swagger UI. The feature allows users to ask about authors, books, publishers, or categories and then ask follow-up questions without repeating the entity names.

## Step-by-Step Testing Process

### Step 1: Start the API Server
1. Run the AseerAlkotb.API project
2. Open Swagger UI in your browser (usually `https://localhost:7207/swagger` or similar)
3. Navigate to the `/api/Rag/ask` endpoint

### Step 2: First Request - Ask About an Author
This will cache the author information in the session.

**Request Setup:**
1. Click on the `/api/Rag/ask` endpoint to expand it
2. Click "Try it out"
3. In the request body, enter:
```json
{
  "question": "من هو نجيب محفوظ؟",
  "limit": 5
}
```

**Execute the Request:**
1. Click "Execute"
2. **IMPORTANT**: In the response headers, look for `X-Session-Id`
3. **Copy this session ID** - you'll need it for the next request
4. Verify the response contains author information about Naguib Mahfouz

**Expected Response:**
- Status: 200 OK
- Response body should contain author biography
- Response headers should include `X-Session-Id: <some-guid>`

### Step 3: Second Request - Ask About Author's Books (Using Session)
This will test if the system uses the cached author information.

**Request Setup:**
1. **Before executing**: In the "Parameters" section, look for the header parameters
2. Add a new header parameter:
   - **Parameter name**: `X-Session-Id`
   - **Value**: The session ID you copied from Step 2
3. In the request body, enter:
```json
{
  "question": "ما هي أشهر كتبه؟",
  "limit": 5
}
```

**Execute the Request:**
1. Click "Execute"
2. Check the response

**Expected Success Behavior:**
- The system should automatically know you're asking about Naguib Mahfouz's books
- Response should list his famous books like "الثلاثية", "أولاد حارتنا", etc.
- Should NOT return the generic "برجاء تحديد اسم المؤلف" message

## Alternative Test Scenarios

### Scenario 2: Book Context Persistence
**Step 1**: Ask about a book
```json
{
  "question": "أريد ملخص كتاب الخيميائي",
  "limit": 5
}
```

**Step 2**: Ask about availability (using same session ID)
```json
{
  "question": "هل هو متاح؟",
  "limit": 5
}
```

### Scenario 3: Publisher Context Persistence
**Step 1**: Ask about a publisher
```json
{
  "question": "معلومات عن دار الشروق",
  "limit": 5
}
```

**Step 2**: Ask about their books (using same session ID)
```json
{
  "question": "ما هي كتبهم المتاحة؟",
  "limit": 5
}
```

## How to Add Headers in Swagger UI

### Method 1: Using the Headers Section
1. Scroll up to find the "Parameters" section
2. Look for header parameters
3. Add `X-Session-Id` with your session ID value

### Method 2: If Headers Section is Not Visible
1. Look for a "Headers" or "Request Headers" section in the UI
2. Add the session header manually
3. Some Swagger UI versions allow you to edit the raw request

### Method 3: Manual Header Addition
If Swagger doesn't show header fields:
1. Look for an "Add Header" button or similar
2. Click it and add:
   - **Name**: `X-Session-Id`
   - **Value**: `<your-session-id-from-step-1>`

## Debugging Tips

### Check Response Headers
Always look at the response headers to confirm:
- `X-Session-Id` is present and consistent
- Session ID matches between requests

### Verify Session Caching
If the second request asks for entity specification, check:
1. **Session ID**: Make sure you're passing the same session ID
2. **Headers**: Confirm the `X-Session-Id` header is included
3. **Question Format**: Ensure the question implies context (use pronouns like "his books", "هو", "كتبه")

### Check Application Logs
Look for these log messages in your application console:
- `"Session {SessionId}: Intent={Intent}, Author={Author}..."`
- `"Detected 'more_by_author' intent from cached context"`

## Common Issues and Solutions

### Issue 1: Generic Response on Follow-up
**Problem**: Second request returns "برجاء تحديد اسم المؤلف"
**Solution**: 
- Verify session ID is being passed correctly
- Check that the question implies context about books/author

### Issue 2: No Session ID in Response
**Problem**: First response doesn't include `X-Session-Id` header
**Solution**: 
- Check if the API server is running the updated code
- Verify the RagController is properly setting response headers

### Issue 3: Session Not Persistent
**Problem**: Each request acts like a new session
**Solution**: 
- Ensure you're copying the exact session ID (including hyphens)
- Verify the header name is exactly `X-Session-Id` (case-sensitive)

## Success Criteria

✅ **Test Passes When:**
1. First request caches the entity (author/book/publisher)
2. Response includes `X-Session-Id` header
3. Second request with same session ID automatically uses cached entity
4. No "please specify" error messages for context-dependent questions
5. Follow-up answers are relevant to the originally mentioned entity

✅ **Example Successful Flow:**
```
Request 1: "من هو نجيب محفوظ؟" 
Response 1: Biography + Session ID

Request 2: "ما هي أشهر كتبه؟" (with Session ID)
Response 2: List of Naguib Mahfouz's books (not generic error)
```

## Notes
- Session data is temporary and stored in memory
- Sessions expire after 30 minutes of inactivity (configurable)
- The system supports Arabic and English questions
- Entity caching works for: Authors, Books, Publishers, Categories