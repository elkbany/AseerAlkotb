# دليل نظام RAG للبحث الذكي في الكتب

## نظرة عامة

تم تطوير نظام RAG (Retrieval-Augmented Generation) للموقع لمساعدة المستخدمين في البحث عن الكتب والحصول على إجابات ذكية بناءً على البيانات المتاحة.

## الميزات الرئيسية

### 1. البحث الذكي في الكتب

- البحث باستخدام embedding vectors للعثور على الكتب الأكثر صلة
- دعم البحث باللغة العربية والإنجليزية
- عرض معلومات الكتاب (السعر، التوفر، عدد الصفحات)

### 2. البحث عن المؤلفين

- البحث عن معلومات المؤلف
- عرض كتب المؤلف
- جلب معلومات إضافية من ويكيبيديا

### 3. البحث في الفئات

- البحث عن كتب فئة معينة
- عرض وصف الفئة والكتب المتاحة

### 4. التوصيات الذكية

- اقتراح كتب مشابهة بناءً على الاستعلام
- ترتيب النتائج حسب التقييم والمبيعات

### 5. معلومات إضافية من الإنترنت

- جلب ملخصات الكتب من Google Books API
- معلومات إضافية عن المؤلفين من ويكيبيديا

## API Endpoints

### 1. طرح سؤال ذكي

```
POST /api/Rag/ask
Content-Type: application/json

{
  "query": "أريد كتاب عن البرمجة"
}
```

### 2. التحقق من توفر كتاب

```
GET /api/Rag/book-availability/{bookTitle}
```

### 3. جلب كتب مؤلف معين

```
GET /api/Rag/author-books/{authorName}
```

### 4. جلب كتب فئة معينة

```
GET /api/Rag/category-books/{categoryName}
```

### 5. جلب ملخص كتاب من الإنترنت

```
GET /api/Rag/book-summary?bookTitle={title}&authorName={author}
```

### 6. الحصول على توصيات

```
GET /api/Rag/recommendations?query={searchQuery}
```

### 7. البحث الذكي

```
GET /api/Rag/smart-search?searchQuery={query}&topK={number}
```

### 8. تحديث Embeddings

```
POST /api/Rag/update-embeddings
```

## أمثلة على الاستخدام

### البحث عن كتاب معين

```json
{
  "query": "كتاب عن الذكاء الاصطناعي"
}
```

### البحث عن مؤلف

```json
{
  "query": "كتب أحمد خالد توفيق"
}
```

### البحث في فئة

```json
{
  "query": "كتب الخيال العلمي"
}
```

### طلب توصيات

```json
{
  "query": "اقترح علي كتب مشابهة لكتاب الأب الغني والأب الفقير"
}
```

## إعداد النظام

### 1. إضافة API Keys

في ملف `appsettings.json`:

```json
{
  "ExternalAPIs": {
    "GoogleBooksApiKey": "YOUR_GOOGLE_BOOKS_API_KEY",
    "WikipediaApiUrl": "https://ar.wikipedia.org/api/rest_v1/page/summary/"
  }
}
```

### 2. إنشاء Migration

```bash
dotnet ef migrations add AddRagTables
dotnet ef database update
```

### 3. تحديث Embeddings

بعد إضافة كتب جديدة، قم بتحديث embeddings:

```bash
POST /api/Rag/update-embeddings
```

## البنية التقنية

### 1. نماذج البيانات

- `BookEmbedding`: لتخزين vector representations للكتب
- `RagQuery`: لتسجيل الاستعلامات والإجابات

### 2. الخدمات

- `EmbeddingService`: لإنشاء ومعالجة embeddings
- `RagService`: لمعالجة الاستعلامات وإنتاج الإجابات
- `ExternalBookService`: للتفاعل مع APIs الخارجية

### 3. خوارزمية البحث

- استخدام Cosine Similarity للبحث عن الكتب المشابهة
- دعم TF-IDF كطريقة بديلة للـ embedding
- إمكانية استخدام ONNX models للـ embedding المتقدم

## تحسينات مستقبلية

1. **استخدام نماذج ONNX متقدمة** للـ embedding
2. **إضافة cache** للنتائج المتكررة
3. **تحسين خوارزمية التوصيات** باستخدام collaborative filtering
4. **إضافة دعم للبحث الصوتي**
5. **تحسين دقة البحث** باستخدام semantic search

## استكشاف الأخطاء

### مشاكل شائعة:

1. **عدم وجود embeddings**: قم بتشغيل `/api/Rag/update-embeddings`
2. **خطأ في API الخارجي**: تحقق من API keys في appsettings.json
3. **بطء في البحث**: تأكد من وجود indexes في قاعدة البيانات

### سجلات النظام:

- يتم تسجيل جميع الاستعلامات في جدول `RagQueries`
- يمكن مراقبة أداء النظام من خلال logs

## الأمان

- جميع endpoints محمية بـ JWT authentication
- لا يتم تخزين معلومات حساسة في embeddings
- استخدام HTTPS لجميع الطلبات الخارجية
