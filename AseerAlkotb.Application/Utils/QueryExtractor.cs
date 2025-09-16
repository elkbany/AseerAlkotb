﻿﻿using System.Text.RegularExpressions;

namespace AseerAlkotb.Application.Utils
{
    public static class QueryExtractor
    {
        // علامات اقتباس عربي/إنجليزي
        private static readonly Regex QuotedTitle = new(@"[\""“”«»‚‘’'`]+(?<t>[^\""“”«»‚‘’'`]{2,})[\""“”«»‚‘’'`]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // "كتاب <عنوان>" أو "عن كتاب <عنوان>"
        private static readonly Regex AfterKitab = new(@"\b(عن\s+)?(?:كتاب|book)\s+(?<t>[^\.,;:()\[\]{}،]{2,})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // "by <author>" (إنجليزي)
        private static readonly Regex ByAuthor = new(@"\bby\s+(?<a>[A-Za-z][A-Za-z\.\-'\s]{1,60})\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // عربي: "لـ<اسم>" / "لـ <اسم>" / "للمؤلف <اسم>" / "للكاتب <اسم>"
        private static readonly Regex ArabicAuthor =
            new(@"\b(?:ل\s*|لـ)\s*(?:ل?كاتب|ل?مؤلف|للمؤلف|للكاتب)?\s*(?<a>[اأإآء-ي][\p{L}\s\.\-']{1,60})",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        // "المؤلف: <اسم>" أو "الكاتب: <اسم>"
        private static readonly Regex LabelAuthor = new(@"\b(?:المؤلف|الكاتب)\s*[:：]\s*(?<a>[اأإآء-يA-Za-z][\p{L}A-Za-z\s\.\-']{1,60})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // دار النشر patterns: "دار <اسم>" أو "منشورات <اسم>" أو "من دار النشر <اسم>"
        private static readonly Regex PublisherPatterns = new(@"\b(?:دار|منشورات|من\s+دار\s+النشر)\s+(?<p>[اأإآء-يA-Za-z][\p{L}A-Za-z\s\.\-']{1,60})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        // "الناشر: <اسم>" أو "publisher: <name>"
        private static readonly Regex LabelPublisher = new(@"\b(?:الناشر|publisher)\s*[:：]\s*(?<p>[اأإآء-يA-Za-z][\p{L}A-Za-z\s\.\-']{1,60})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static (string? Title, string? Author, string? Publisher) ExtractAdvanced(string question)
        {
            if (string.IsNullOrWhiteSpace(question)) return (null, null, null);
            var q = question.Trim();

            // 1) عنوان داخل اقتباس
            var m = QuotedTitle.Match(q);
            if (m.Success) return (Clean(m.Groups["t"].Value), FindAuthor(q), FindPublisher(q));

            // 2) "كتاب <عنوان>"
            m = AfterKitab.Match(q);
            if (m.Success) return (Clean(m.Groups["t"].Value), FindAuthor(q), FindPublisher(q));

            // 3) لو مفيش عنوان، رجّع بس المؤلف والناشر لو موجودين
            return (null, FindAuthor(q), FindPublisher(q));
        }

        public static (string? Title, string? Author) Extract(string question)
        {
            if (string.IsNullOrWhiteSpace(question)) return (null, null);
            var q = question.Trim();

            // 1) عنوان داخل اقتباس
            var m = QuotedTitle.Match(q);
            if (m.Success) return (Clean(m.Groups["t"].Value), FindAuthor(q));

            // 2) "كتاب <عنوان>"
            m = AfterKitab.Match(q);
            if (m.Success) return (Clean(m.Groups["t"].Value), FindAuthor(q));

            // 3) لو مفيش عنوان، رجّع بس المؤلف لو موجود
            return (null, FindAuthor(q));
        }

        private static string? FindAuthor(string q)
        {
            var m = ByAuthor.Match(q);
            if (m.Success) return Clean(m.Groups["a"].Value);

            m = ArabicAuthor.Match(q);
            if (m.Success) return Clean(m.Groups["a"].Value);

            m = LabelAuthor.Match(q);
            if (m.Success) return Clean(m.Groups["a"].Value);

            return null;
        }

        private static string? FindPublisher(string q)
        {
            var m = PublisherPatterns.Match(q);
            if (m.Success) return Clean(m.Groups["p"].Value);

            m = LabelPublisher.Match(q);
            if (m.Success) return Clean(m.Groups["p"].Value);

            return null;
        }

        private static string Clean(string x)
        {
            var s = x.Trim();

            // تطبيع بسيط
            s = s.Replace("ـ", ""); // إزالة الكشيدة
            s = s.Replace("لـ", "ل"); // توحيد لـ → ل

            // شيل الكلمات المفتاحية لو ظهرت في البداية
            var removePrefixes = new[]
            {
        "كتاب", "الكتاب",
        "الكاتب", "المؤلف",
        "للكاتب", "لكاتب", "للمؤلف", "لمؤلف",
        "الكاتب:", "المؤلف:",
        "بقلم", "بقلم:", "تأليف", "تأليف:",
        "دار", "منشورات", "الناشر", "الناشر:",
        "من دار النشر", "دار النشر"
    };

            foreach (var prefix in removePrefixes)
            {
                if (s.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    s = s.Substring(prefix.Length).Trim();
                    break;
                }
            }

            // قص خلف أي فواصل ثقيلة
            var cut = new[] { "،", ",", ";", "؛", ".", ":", "—", "-", "؟", "?" };
            foreach (var c in cut)
            {
                var idx = s.IndexOf(c, StringComparison.Ordinal);
                if (idx > 1)
                {
                    s = s[..idx];
                    break;
                }
            }

            return s.Trim(' ', '\"', '“', '”', '«', '»', '\'', '`');
        }


    }
}
