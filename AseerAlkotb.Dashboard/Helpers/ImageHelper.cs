// AseerAlkotb.Dashboard/Helpers/ImageHelper.cs
using System;

namespace AseerAlkotb.Dashboard.Helpers
{
    public static class ImageHelper
    {
        public static string Resolve(string url, string apiBase, string fallback = "/images/default-user.png")
        {
            if (string.IsNullOrWhiteSpace(url)) return fallback;
            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return url;
            apiBase = apiBase?.TrimEnd('/') ?? "";
            url = url.StartsWith("/") ? url : "/" + url;
            return apiBase + url;
        }
    }
}
