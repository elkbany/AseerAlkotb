## RAG Session Memory Testing Guide

This document explains how to test the new session-based memory functionality in your RAG system.

### How It Works

The system now remembers your conversation during a session:

- Each user gets a unique session ID
- Questions and answers are cached temporarily (30 minutes by default)
- Bot provides context-aware responses based on conversation history
- Repeated questions get smarter responses

### API Usage

#### 1. First Request (New Session)

```http
POST /api/rag/ask
Content-Type: application/json

{
  "question": "عايز ملخص كتاب الخيميائي",
  "limit": 5
}
```

**Response Headers:**

```
X-Session-Id: 12345678-abcd-1234-efgh-123456789012
```

#### 2. Follow-up Requests (Same Session)

```http
POST /api/rag/ask
Content-Type: application/json
X-Session-Id: 12345678-abcd-1234-efgh-123456789012

{
  "question": "مين المؤلف بتاع الكتاب ده؟",
  "limit": 5
}
```

The bot will remember you asked about "الخيميائي" and provide contextual response.

#### 3. Testing Repeated Questions

```http
POST /api/rag/ask
Content-Type: application/json
X-Session-Id: 12345678-abcd-1234-efgh-123456789012

{
  "question": "عايز ملخص كتاب الخيميائي",
  "limit": 5
}
```

Bot response: "أتذكر إنك سألت عن هذا مؤخراً. [previous answer]"

### Configuration

In `appsettings.json`:

```json
{
  "SessionMemory": {
    "TimeoutMinutes": 30, // Session expires after 30 minutes
    "MaxMessages": 20, // Keep last 20 messages per session
    "CleanupIntervalMinutes": 15 // Clean expired sessions every 15 minutes
  }
}
```

### Key Features

1. **Session Continuity**: Bot remembers previous books, authors, and topics you discussed
2. **Smart Repetition Handling**: Detects repeated questions and provides contextual responses
3. **Bilingual Support**: Works in both Arabic and English with memory
4. **Automatic Cleanup**: Old sessions are automatically cleaned up
5. **Context-Aware Synthesis**: Uses conversation history to provide better answers

### Example Conversation Flow

```
User: "عايز ترشيحات روايات"
Bot: "ترشيحات ضمن «روايات»: زقاق المدق، الأسود يليق بك، مئة عام من العزلة..."

User: "معلومات عن نجيب محفوظ"
Bot: "بناءً على محادثتنا السابقة - تحدثنا عن تصنيف: روايات. نجيب محفوظ كاتب مصري حائز على جائزة نوبل..."

User: "كتب أخرى له"
Bot: "بناءً على محادثتنا السابقة - تكلمنا عن المؤلف: نجيب محفوظ. كتب أخرى لنفس المؤلف: بين القصرين، قصر الشوق، السكرية..."
```

### Testing Tips

1. **Use Postman or similar tool** to test with session headers
2. **Test session expiry** by waiting 30+ minutes
3. **Test bilingual conversations** (mix Arabic/English)
4. **Test repeated questions** within 10 minutes
5. **Test conversation context** by asking follow-up questions

### Memory Limitations

- **Temporary only**: Memory is cleared when application restarts
- **Session-based**: Each session is independent
- **Size limits**: Max 20 messages per session (configurable)
- **Time limits**: Sessions expire after 30 minutes of inactivity

### Troubleshooting

- **No context in responses?** Check if X-Session-Id header is being sent
- **Memory not working?** Check logs for SessionMemoryService initialization
- **Performance issues?** Adjust MaxMessages in configuration
