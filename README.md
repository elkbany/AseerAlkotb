# 📚 AseerAlkotb

**AseerAlkotb** is an **AI-powered bilingual (Arabic/English)** book e-commerce platform that combines modern ASP.NET Core Web API architecture with intelligent search powered by **RAG (Retrieval-Augmented Generation)** and **Google Gemini AI**. It enables users to explore books naturally — whether asking for summaries, author info, or similar recommendations — while offering a full e-commerce experience.

---

## 🚀 Project Overview

- **AseerAlkotb** is a bilingual (Arabic/English) book management and e-commerce system.  
- Built using **.NET 9.0** and **ASP.NET Core Web API**.  
- Implements **RAG (Retrieval-Augmented Generation)** for context-aware book search.  
- Uses **Google Gemini AI** for intent understanding and response generation.  
- Integrates **Paymob** for multi-channel payment support (cards, wallets, cash).  

---

## ✨ Key Features

### 1. 🤖 AI-Powered Smart Search
- Supports **natural language queries** in both Arabic and English.  
- Detects **10 search intents**:
  - `summary`, `availability`, `price`, `author_bio`, `more_by_author`,  
    `category_recs`, `similar_to_title`, `publisher_info`, `publisher_books`, `general_recs`
- Uses **768-dimensional vector embeddings** for semantic similarity.  
- Maintains **multi-turn conversational context** with a 30-minute session memory timeout.  

### 2. 🛒 E-Commerce Functionality
- Shopping cart and wishlist management.  
- Complete order lifecycle: **Pending → Approved → Shipped → Delivered → Cancelled**.  
- **Paymob** integration with HMAC validation for secure payments.  
- **Cash on Delivery** option supported.  

### 3. 📚 Content Management
- Book catalog with **categories, authors, and publishers**.  
- User **reviews and ratings** with like functionality.  
- **Author following** and **quote management** system.  

### 4. 🌍 Localization
- Full **bilingual support** (Arabic/English).  
- Automatic **language detection and translation**.  
- Localized resources for all entities and UI components.  

---

## 🧠 Technology Stack

| Layer | Technology |
|-------|-------------|
| **Backend** | ASP.NET Core 9.0, Entity Framework Core |
| **Database** | SQL Server (with lazy loading proxies) |
| **AI / ML** | Google Gemini API (`gemini-1.5-flash`, `text-embedding-004`) |
| **Authentication** | ASP.NET Identity + JWT Bearer Tokens |
| **Payments** | Paymob (card, wallet, COD) |
| **Image Storage** | Cloudinary |
| **Email Service** | Gmail SMTP |

---

## ⚙️ Prerequisites

Before running the project, ensure you have:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Google Gemini API Key](https://aistudio.google.com/)
- [Paymob Account](https://www.paymob.com/)
- [Cloudinary Account](https://cloudinary.com/)

---

## 🔧 Configuration

Edit **`appsettings.json`** with the following:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your-SQL-Connection-String",
    "Shared": "Your-Secondary-Connection-String"
  },
  "Gemini": {
    "ApiKey": "YOUR_GEMINI_API_KEY",
    "Models": {
      "Chat": "gemini-1.5-flash",
      "Embedding": "text-embedding-004"
    }
  },
  "Paymob": {
    "ApiKey": "YOUR_PAYMOB_API_KEY",
    "SecretKey": "YOUR_SECRET",
    "PublicKey": "YOUR_PUBLIC_KEY",
    "IntegrationIds": {
      "Card": "123456",
      "Wallet": "654321"
    },
    "HMAC": "YOUR_HMAC_SECRET"
  },
  "JWT": {
    "SecretKey": "YOUR_JWT_SECRET",
    "AudienceIP": "YOUR_APP_URL",
    "IssuerIP": "YOUR_BACKEND_URL"
  },
  "EmailSettings": {
    "SMTP": "smtp.gmail.com",
    "Port": 587,
    "Username": "youremail@gmail.com",
    "Password": "app-password"
  },
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  },
  "RAG": {
    "SummarizeFromDescription": true,
    "SessionMemoryTimeoutMinutes": 30
  }
}
```

---

## 🏗️ Installation & Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/yourusername/AseerAlkotb.git
   cd AseerAlkotb
   ```

2. **Update connection strings** in `appsettings.json`.

3. **Apply migrations:**
   ```bash
   dotnet ef database update
   ```

4. **Add your API keys** (Gemini, Paymob, Cloudinary).

5. **Run the API:**
   ```bash
   cd AseerAlkotb.API
   dotnet run
   ```

6. **Run the Dashboard:**
   ```bash
   cd AseerAlkotb.Dashboard
   dotnet run
   ```

---

## 🧩 API Architecture

- RESTful API with **controller-based routing**.  
- **Repository Pattern** + **Unit of Work**.  
- **Service Layer** encapsulates business logic.  
- **FluentValidation** for request models.  
- **Middleware** for rate limiting, CORS, and exception handling.  
- **Swagger** documentation for API testing.  

---

## 🧬 RAG System Details

| Component | Description |
|------------|--------------|
| **Question Routing** | Gemini-based intent classification with entity extraction |
| **Embedding Generation** | 768-dimensional vectors for semantic similarity |
| **Similarity Search** | Cosine similarity with in-memory caching |
| **Answer Synthesis** | Context-aware responses with source citations |
| **Session Memory** | 30-min conversational state (ConcurrentDictionary) |
| **Concurrency Control** | Semaphore-based rate limiting (max 3 Gemini calls) |
| **Retry Logic** | Exponential backoff for transient errors |

---

## 🗃️ Database Schema

Key entities include:

- `Users`, `Books`, `Authors`, `Publishers`, `Categories`  
- `Orders`, `Payments`, `Reviews`, `CartItems`, `Wishlist`, `Quotes`  
- `BookEmbeddings` for AI-powered search  

---

## 🔒 Security Features

- **JWT Authentication** with email confirmation.  
- **Role-based Authorization** (`Client`, `Admin`).  
- **HMAC Validation** for Paymob payment callbacks.  
- **IP Whitelisting** for Paymob webhooks.  
- **Endpoint Rate Limiting** and **encrypted connections**.  

---

## 🌐 Frontend Integration

- CORS configured for **Vercel** or similar deployments.  
- Session-based chat for follow-up queries.  
- API Base URLs defined for frontend communication.  

---

## 📘 Documentation

Additional project guides:

- [`RAG_TESTING_GUIDE.md`](./docs/RAG_TESTING_GUIDE.md)  
- [`RAG_SYSTEM_GUIDE.md`](./docs/RAG_SYSTEM_GUIDE.md)  
- [`SWAGGER_SESSION_TESTING_GUIDE.md`](./docs/SWAGGER_SESSION_TESTING_GUIDE.md)  
- [`MANUAL_SESSION_TEST_CHECKLIST.md`](./docs/MANUAL_SESSION_TEST_CHECKLIST.md)  
- [`ORDER_PAYMENT_FLOW_DOCUMENTATION.md`](./docs/ORDER_PAYMENT_FLOW_DOCUMENTATION.md)  

---

## 📄 License

Specify your license here — for example:  
[MIT License](./LICENSE)

---

## 👥 Contributors

| Name | Role | Contact |
|------|------|----------|
| Mahmoud Mohamed Amin | Lead Developer / AI Engineer | [aseerelkotb@gmail.com](mailto:aseerelkotb@gmail.com) |
| [List others here] | — | — |

---

## 📬 Contact

For inquiries or collaborations:  
📧 **Mahmoudelkbany00@gmail.com**

---

> _"AseerAlkotb — bridging the world of Arabic literature with modern AI."_
