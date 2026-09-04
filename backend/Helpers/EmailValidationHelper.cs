using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IMS.Backend.Helpers
{
    public static class EmailValidationHelper
    {
        public const int EmailMaxLength = 254;

        private static readonly HashSet<string> ValidTlds = new(StringComparer.OrdinalIgnoreCase)
        {
            "com", "in", "org", "net", "edu", "gov", "io", "co", "info", "biz", "tech",
            "app", "dev", "store", "online", "me", "site", "ca", "uk", "au", "us", "de",
            "fr", "jp", "sg", "ae", "cn", "ru", "br", "nl", "se", "no", "fi", "dk", "pl",
            "it", "es", "mx", "za", "nz", "ch", "at", "be", "ph", "id", "my", "th", "vn",
            "live", "cloud", "digital", "global", "systems", "solutions", "agency", "group",
            "services", "co.in", "net.in", "org.in", "edu.in", "gov.in", "ac.in", "co.uk",
            "com.au", "co.jp", "or.jp", "ne.jp", "ac.uk", "gov.uk", "co.za"
        };

        private static readonly HashSet<string> TypoTlds = new(StringComparer.OrdinalIgnoreCase)
        {
            "cm", "c", "coom", "comm", "commm", "ccommmm", "con", "cmm", "gma", "gmai", "gamil"
        };

        private static readonly Dictionary<string, string> CommonDomainTypos = new(StringComparer.OrdinalIgnoreCase)
        {
            { "gmail.cm", "gmail.com" },
            { "gmail.co", "gmail.com" },
            { "gmail.commm", "gmail.com" },
            { "gmail.comm", "gmail.com" },
            { "gmail.coom", "gmail.com" },
            { "gmai.com", "gmail.com" },
            { "gmai.co", "gmail.com" },
            { "gamil.com", "gmail.com" },
            { "gamil.co", "gmail.com" },
            { "gmial.com", "gmail.com" },
            { "gmial.co", "gmail.com" },
            { "gmaill.com", "gmail.com" },
            { "yahoo.cm", "yahoo.com" },
            { "yahoo.co", "yahoo.com" },
            { "yaho.com", "yahoo.com" },
            { "hotmail.cm", "hotmail.com" },
            { "hotmail.co", "hotmail.com" },
            { "hotmial.com", "hotmail.com" },
            { "outlook.cm", "outlook.com" },
            { "outlook.co", "outlook.com" },
            { "outlok.com", "outlook.com" }
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
            }

            var lowerDomain = domainPart.ToLowerInvariant();
            if (CommonDomainTypos.ContainsKey(lowerDomain))
            {
                return false;
            }

            var tld = domainLabels[^1].ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(tld) || !Regex.IsMatch(tld, @"^[a-z]+$") || tld.Length < 2 || tld.Length > 24)
            {
                return false;
            }

            if (TypoTlds.Contains(tld))
            {
                return false;
            }

            if (domainLabels.Length >= 3)
            {
                var lastTwo = $"{domainLabels[^2].ToLowerInvariant()}.{tld}";
                if (Regex.IsMatch(lastTwo, @"^[a-z]{2,4}\.[a-z]{2,4}$"))
                {
                    if (!ValidTlds.Contains(lastTwo) && !ValidTlds.Contains(tld))
                    {
                        return false;
                    }
                    if (Regex.IsMatch(lastTwo, @"^[a-z]{2}\.[a-z]{2}$") && !ValidTlds.Contains(lastTwo))
                    {
                        return false;
                    }
                }
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
