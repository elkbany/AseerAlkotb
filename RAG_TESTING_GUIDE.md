# دليل اختبار نظام RAG

## نظرة عامة

تم بناء نظام RAG (Retrieval-Augmented Generation) كامل لموقع الكتب. هذا الدليل يوضح كيفية اختبار النظام والتأكد من عمله بشكل صحيح.

## المتطلبات

- .NET 9.0
- SQL Server
- Visual Studio أو VS Code

## خطوات الاختبار

### 1. تشغيل المشروع

```bash
cd AseerAlkotb.API
dotnet run
```

### 2. اختبار الـ Endpoints

#### أ) معالجة الاستعلامات الذكية

```http
POST http://localhost:5234/api/rag/ask
Content-Type: application/json

{
  "query": "أريد كتاب عن البرمجة"
}
```

#### ب) جلب ملخص كتاب من الإنترنت

```http
GET http://localhost:5234/api/rag/book-summary?bookTitle=Clean Code&authorName=Robert Martin
```

#### ج) البحث الذكي

```http
GET http://localhost:5234/api/rag/smart-search?searchQuery=برمجة&topK=5
```

#### د) جلب كتب الكاتب

```http
GET http://localhost:5234/api/rag/author-books/أحمد خالد توفيق
```

#### هـ) جلب كتب الفئة

```http
GET http://localhost:5234/api/rag/category-books/البرمجة
```

#### و) التحقق من توفر الكتاب

```http
GET http://localhost:5234/api/rag/book-availability/كتاب البرمجة
```

#### ز) جلب التوصيات

```http
GET http://localhost:5234/api/rag/recommendations?query=أحب كتب الخيال العلمي
```

#### ح) تحديث Embeddings

```http
POST http://localhost:5234/api/rag/update-embeddings
```

### 3. اختبار APIs الخارجية

#### Google Books API

النظام يستخدم Google Books API للحصول على معلومات الكتب:

- URL: `https://www.googleapis.com/books/v1/volumes`
- لا يتطلب API key للاستخدام الأساسي

#### Wikipedia API

النظام يستخدم Wikipedia API للحصول على ملخصات:

- URL: `https://ar.wikipedia.org/api/rest_v1/page/summary/`
- لا يتطلب API key

### 4. اختبار قاعدة البيانات

#### جداول جديدة:

- `BookEmbeddings`: لتخزين embeddings الكتب
- `RagQueries`: لتسجيل استعلامات المستخدمين

#### التحقق من البيانات:

```sql
SELECT COUNT(*) FROM BookEmbeddings;
SELECT COUNT(*) FROM RagQueries;
```

## النتائج المتوقعة

### 1. معالجة الاستعلامات

- تصنيف تلقائي للاستعلامات (كتب، مؤلفين، فئات، توصيات)
- إجابات مناسبة باللغة العربية
- اقتراحات للكتب ذات الصلة

### 2. APIs الخارجية

- جلب ملخصات من Google Books
- جلب معلومات من Wikipedia
- معالجة الأخطاء بشكل مناسب

### 3. البحث الذكي

- استخدام embeddings للبحث المشابه
- نتائج مرتبة حسب التشابه
- دعم البحث باللغة العربية

## استكشاف الأخطاء

### مشاكل شائعة:

1. **المشروع لا يبدأ**

   - تحقق من connection string
   - تأكد من تحديث قاعدة البيانات

2. **APIs الخارجية لا تعمل**

   - تحقق من الاتصال بالإنترنت
   - تأكد من صحة URLs

3. **البحث لا يعطي نتائج**
   - تأكد من وجود بيانات في قاعدة البيانات
   - تحقق من تحديث embeddings

## تحسينات مستقبلية

1. **تحسين الـ Embeddings**

   - استخدام ML models أكثر تطوراً
   - دعم لغات متعددة

2. **تحسين الأداء**

   - إضافة caching
   - تحسين استعلامات قاعدة البيانات

3. **إضافة ميزات جديدة**
   - تحليل المشاعر
   - توصيات شخصية
   - دعم الصور

## الدعم

للحصول على المساعدة أو الإبلاغ عن مشاكل، يرجى التواصل مع فريق التطوير.
