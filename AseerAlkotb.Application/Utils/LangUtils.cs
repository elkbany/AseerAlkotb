using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Utils
{
    public enum Lang { Arabic, English }

    public static class LangUtils
    {
        private static readonly Regex HasArabic = new(@"\p{IsArabic}", RegexOptions.Compiled);
        private static readonly Regex HasLatin = new(@"[A-Za-z]", RegexOptions.Compiled);

        public static Lang Detect(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return Lang.Arabic;
            var t = text;
            var ar = HasArabic.IsMatch(t);
            var en = HasLatin.IsMatch(t);
            if (ar && !en) return Lang.Arabic;
            if (en && !ar) return Lang.English;
            return char.IsLetter(t.Trim()[0]) && t.Trim()[0] <= 127 ? Lang.English : Lang.Arabic;
        }
    }
}
