using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Localization.Resources
{
    public class SharedResources
    {
    }

    public static class ResxResourceHelper
    {
        public static bool HasSharedResource(string key, string culture)
        {
            return GetSharedResourceOrNull(key, culture) != null;
        }

        public static void EnsureCategoryLocalization(int id, string? nameAr, string? nameEn, string? descAr, string? descEn)
        {
            var keyName = $"Category_{id}_Name";
            var keyDesc = $"Category_{id}_Description";

            // Names
            if (!HasSharedResource(keyName, "ar") && !string.IsNullOrWhiteSpace(nameAr))
            {
                UpsertSharedResource(keyName, nameAr!, "ar");
            }
            if (!HasSharedResource(keyName, "en"))
            {
                var valEn = string.IsNullOrWhiteSpace(nameEn) ? (nameAr ?? string.Empty) : nameEn!;
                UpsertSharedResource(keyName, valEn, "en");
            }

            // Descriptions
            if (!string.IsNullOrWhiteSpace(descAr) || !string.IsNullOrWhiteSpace(descEn))
            {
                if (!HasSharedResource(keyDesc, "ar"))
                {
                    var valAr = string.IsNullOrWhiteSpace(descAr) ? (descEn ?? string.Empty) : descAr!;
                    UpsertSharedResource(keyDesc, valAr, "ar");
                }
                if (!HasSharedResource(keyDesc, "en"))
                {
                    var valEn = string.IsNullOrWhiteSpace(descEn) ? (descAr ?? string.Empty) : descEn!;
                    UpsertSharedResource(keyDesc, valEn, "en");
                }
            }
        }

        public static string? GetSharedResourceOrNull(string key, string culture)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            if (string.IsNullOrWhiteSpace(culture)) culture = "en";

            var projectRoot = FindLocalizationProjectRoot();
            if (projectRoot == null) return null;

            var resxPath = Path.Combine(projectRoot, "Resources", $"SharedResources.{culture}.resx");
            if (!File.Exists(resxPath)) return null;

            try
            {
                var doc = System.Xml.Linq.XDocument.Load(resxPath);
                var root = doc.Element("root");
                if (root == null) return null;
                var node = root.Elements("data")
                    .FirstOrDefault(x => string.Equals((string?)x.Attribute("name"), key, StringComparison.OrdinalIgnoreCase));
                var value = node?.Element("value")?.Value;
                return string.IsNullOrWhiteSpace(value) ? null : value;
            }
            catch { return null; }
        }

        public static void UpsertSharedResource(string key, string value, string culture)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            if (string.IsNullOrWhiteSpace(culture)) culture = "en";

            var projectRoot = FindLocalizationProjectRoot();
            if (projectRoot == null) return;

            var resxPath = Path.Combine(projectRoot, "Resources", $"SharedResources.{culture}.resx");
            Directory.CreateDirectory(Path.GetDirectoryName(resxPath)!);

            // Load or create basic resx XML
            System.Xml.Linq.XDocument doc;
            if (File.Exists(resxPath))
            {
                doc = System.Xml.Linq.XDocument.Load(resxPath);
            }
            else
            {
                doc = new System.Xml.Linq.XDocument(
                    new System.Xml.Linq.XElement("root",
                        new System.Xml.Linq.XElement("resheader",
                            new System.Xml.Linq.XAttribute("name", "resmimetype"),
                            new System.Xml.Linq.XElement("value", "text/microsoft-resx")
                        ),
                        new System.Xml.Linq.XElement("resheader",
                            new System.Xml.Linq.XAttribute("name", "version"),
                            new System.Xml.Linq.XElement("value", "2.0")
                        ),
                        new System.Xml.Linq.XElement("resheader",
                            new System.Xml.Linq.XAttribute("name", "reader"),
                            new System.Xml.Linq.XElement("value", "System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
                        ),
                        new System.Xml.Linq.XElement("resheader",
                            new System.Xml.Linq.XAttribute("name", "writer"),
                            new System.Xml.Linq.XElement("value", "System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
                        )
                    )
                );
            }

            var root = doc.Element("root")!;
            // Find existing data node
            var existing = root.Elements("data")
                .FirstOrDefault(x => string.Equals((string?)x.Attribute("name"), key, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                existing = new System.Xml.Linq.XElement("data",
                    new System.Xml.Linq.XAttribute("name", key),
                    new System.Xml.Linq.XAttribute(System.Xml.Linq.XNamespace.Xml + "space", "preserve"),
                    new System.Xml.Linq.XElement("value", value ?? string.Empty)
                );
                root.Add(existing);
            }
            else
            {
                var valueElement = existing.Element("value");
                if (valueElement == null)
                {
                    existing.Add(new System.Xml.Linq.XElement("value", value ?? string.Empty));
                }
                else
                {
                    valueElement.Value = value ?? string.Empty;
                }
            }

            doc.Save(resxPath);
        }

        private static string? FindLocalizationProjectRoot()
        {
            try
            {
                var current = AppContext.BaseDirectory;
                // Walk up to find AseerAlkotb.Localization project folder
                for (int i = 0; i < 6 && current != null; i++)
                {
                    var candidate = Path.Combine(current, "AseerAlkotb.Localization");
                    if (Directory.Exists(candidate)) return candidate;
                    current = Directory.GetParent(current)?.FullName;
                }
            }
            catch { }
            return null;
        }
    }
}
