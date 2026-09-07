using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IMS.Backend.Helpers
{
    public static class EmailValidationHelper
    {
        public const int EmailMaxLength = 150;

        private static readonly HashSet<string> ValidTlds = new(StringComparer.OrdinalIgnoreCase)
        {
            "com", "org", "net", "edu", "gov", "mil", "int", "info", "biz", "in", "io", "ai",
            "app", "dev", "tech", "store", "online", "site", "xyz", "me", "tv", "cc", "mobi", "asia",
            "name", "pro", "tel", "travel", "museum", "uk", "us", "ca", "de", "fr", "jp", "cn", "nl",
            "se", "no", "fi", "es", "it", "ru", "mx", "br", "za", "sg", "hk", "tw", "kr", "nz", "ch",
            "at", "be", "dk", "pl", "pt", "cz", "ro", "gr", "hu", "ie", "il", "my", "ph", "th", "vn",
            "id", "ae", "sa", "cl", "ar", "pe", "au", "cloud", "digital", "global",
            "life", "live", "media", "news", "space", "today", "world", "works", "zone",
            "design", "studio", "agency", "solutions", "services", "systems", "network", "company",
            "management", "center", "directory", "shop", "software", "technology", "academy", "education",
            "foundation", "institute", "international", "organization", "ltd", "corp", "enterprises"
        };

        private static readonly HashSet<string> ValidMultiPartTlds = new(StringComparer.OrdinalIgnoreCase)
        {
            "co.in", "net.in", "org.in", "gen.in", "ind.in", "edu.in", "gov.in", "ac.in",
            "co.uk", "org.uk", "me.uk", "ltd.uk", "plc.uk", "ac.uk", "gov.uk",
            "com.au", "net.au", "org.au", "edu.au", "gov.au",
            "co.jp", "or.jp", "ne.jp", "ac.jp", "go.jp",
            "co.za", "org.za", "net.za", "ac.za", "gov.za",
            "co.nz", "net.nz", "org.nz", "ac.nz", "govt.nz",
            "com.sg", "net.sg", "org.sg", "edu.sg", "gov.sg",
            "co.id", "net.id", "or.id", "ac.id", "go.id",
            "com.my", "net.my", "org.my", "edu.my", "gov.my",
            "com.br", "net.br", "org.br",
            "com.mx", "net.mx", "org.mx",
            "com.ar", "net.ar", "org.ar",
            "com.tr", "net.tr", "org.tr"
        };

        private static readonly HashSet<string> TypoTlds = new(StringComparer.OrdinalIgnoreCase)
        {
            "co", "cm", "c", "coom", "comm", "commm", "ccommmm", "con", "cmm", "gma", "gmai", "gamil", "cmo"
        };

        private static readonly HashSet<string> CommonDomainTypos = new(StringComparer.OrdinalIgnoreCase)
        {
            "gmail.cm", "gmail.co", "gmail.comm", "gmail.commm", "gmail.coom",
            "gmai.com", "gmai.co", "gamil.com", "gamil.co", "gmial.com", "gmial.co", "gmaill.com",
            "yahoo.cm", "yahoo.co", "yaho.com",
            "hotmail.cm", "hotmail.co", "hotmial.com",
            "outlook.cm", "outlook.co", "outlok.com"
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

            var mainDomain = domainLabels[0];
            if (mainDomain.Length < 2) return false;

            if (CommonDomainTypos.Contains(domainPart)) return false;

            var tld = domainLabels[^1].ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(tld) || !Regex.IsMatch(tld, @"^[a-z]+$") || tld.Length < 2)
            {
                return false;
            }

            // Check multi-part TLD if domain has 3 or more parts (e.g. farmti.co.in, supplier.co.uk)
            if (domainLabels.Length >= 3)
            {
                var multiTld = $"{domainLabels[^2].ToLowerInvariant()}.{tld}";
                if (ValidMultiPartTlds.Contains(multiTld))
                {
                    return true;
                }
            }

            if (TypoTlds.Contains(tld))
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
