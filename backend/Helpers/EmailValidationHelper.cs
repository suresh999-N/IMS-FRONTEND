using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace IMS.Backend.Helpers
{
    public static class EmailValidationHelper
    {
        public const int EmailMaxLength = 150;

        private static readonly HashSet<string> ValidTlds = new(StringComparer.OrdinalIgnoreCase)
        {
            "com", "org", "net", "edu", "gov", "mil", "int", "info", "biz", "co", "in", "io", "ai",
            "app", "dev", "tech", "store", "online", "site", "xyz", "me", "tv", "cc", "mobi", "asia",
            "name", "pro", "tel", "travel", "museum", "uk", "us", "ca", "de", "fr", "jp", "cn", "nl",
            "se", "no", "fi", "es", "it", "ru", "mx", "br", "za", "sg", "hk", "tw", "kr", "nz", "ch",
            "at", "be", "dk", "pl", "pt", "cz", "ro", "gr", "hu", "ie", "il", "my", "ph", "th", "vn",
            "id", "ae", "sa", "cl", "ar", "pe", "cloud", "digital", "email", "group", "help", "global",
            "life", "live", "link", "media", "news", "space", "today", "world", "works", "zone",
            "design", "studio", "agency", "solutions", "services", "systems", "network", "company",
            "management", "center", "directory", "shop", "blog", "club", "fun", "icu", "one", "top",
            "vip", "work", "fit", "art", "law", "pub", "bar", "ink", "win", "bid", "cam", "run", "red",
            "ren", "kim", "mom", "men", "dad", "day", "fan", "foo", "gop", "how", "moe", "new", "now",
            "ooo", "owl", "rip", "sky", "tax", "tea", "uno", "wtf", "zip", "berlin", "london", "nyc",
            "tokyo", "paris", "amsterdam", "software", "technology", "systems", "academy", "education",
            "foundation", "institute", "international", "organization"
        };

        public static bool IsValidEmail(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var trimmed = value.Trim();

            if (trimmed.Length > EmailMaxLength || Regex.IsMatch(trimmed, @"\s") || trimmed.Contains(".."))
            {
                return false;
            }

            var parts = trimmed.Split('@');
            if (parts.Length != 2) return false;

            var localPart = parts[0];
            var domainPart = parts[1];

            if (string.IsNullOrWhiteSpace(localPart) || string.IsNullOrWhiteSpace(domainPart)) return false;
            if (localPart.Length < 1 || localPart.Length > 64) return false;
            if (localPart.StartsWith(".") || localPart.EndsWith(".")) return false;
            if (!Regex.IsMatch(localPart, @"^[a-zA-Z0-9._%+-]+$")) return false;

            if (domainPart.StartsWith(".") || domainPart.EndsWith(".") || domainPart.StartsWith("-") || domainPart.EndsWith("-"))
            {
                return false;
            }

            var domainLabels = domainPart.Split('.');
            if (domainLabels.Length < 2) return false;

            foreach (var label in domainLabels)
            {
                if (string.IsNullOrWhiteSpace(label) || label.StartsWith("-") || label.EndsWith("-") ||
                    !Regex.IsMatch(label, @"^[a-zA-Z0-9-]+$") || label.Length > 63)
                {
                    return false;
                }

                // Reject 4 or more repeated identical characters in any domain label
                if (Regex.IsMatch(label, @"([a-zA-Z0-9])\1{3,}"))
                {
                    return false;
                }
            }

            var tld = domainLabels[^1].ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(tld) || !Regex.IsMatch(tld, @"^[a-z]+$") || tld.Length < 2)
            {
                return false;
            }

            if (!ValidTlds.Contains(tld))
            {
                return false;
            }

            return true;
        }

        public static bool IsValidName(string? value, int min = 2, int max = 50)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var trimmed = value.Trim();

            if (trimmed.Length < min || trimmed.Length > max) return false;
            if (!Regex.IsMatch(trimmed, @"^[a-zA-Z\p{L}]+(?:\s[a-zA-Z\p{L}]+)*$")) return false;

            var words = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Any(w => w.Length > 15)) return false;
            if (words.Any(w => Regex.IsMatch(w, @"[^aeiouyAEIOUY\p{L}]{5,}"))) return false;

            return true;
        }
    }
}
