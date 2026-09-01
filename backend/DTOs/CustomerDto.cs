using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IMSBackend.DTOs
{
    public class CustomerDto : IValidatableObject
    {
        private static readonly Regex EmailRegex = new(
            @"^[a-z0-9](?:[a-z0-9._%+-]{0,62}[a-z0-9])?@(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\.)+[a-z]{2,24}$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex GstRegex = new(
            @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z][A-Z0-9]Z[A-Z0-9]$",
            RegexOptions.Compiled);
        private static readonly Regex CustomerNameRegex = new(
            @"^[A-Za-z\s]+$",
            RegexOptions.Compiled);

        [RegularExpression(
            @"^[A-Za-z\s]+$",
            ErrorMessage = "Name can contain only letters and spaces.")]
        [Required(ErrorMessage = "Customer name is required.")]
        [MinLength(2, ErrorMessage = "Customer name must be at least 2 characters.")]
        public string Name { get; set; } = string.Empty;

        public string? CustomerCode { get; set; }

        public string? Company { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? GstNumber { get; set; }

        public string? TaxNumber { get; set; }

        public string? PanNumber { get; set; }

        public string? Notes { get; set; }

        public string? Status { get; set; }

        public List<CustomerContactDto> Contacts { get; set; } = new();

        public List<CustomerAddressDto> Addresses { get; set; } = new();

        public CustomerPaymentTermDto? PaymentTerms { get; set; }

        public List<CustomerBankDetailDto> BankDetails { get; set; } = new();

        public List<CustomerBankDetailDto> BankAccounts { get; set; } = new();

        public List<CustomerActivityDto> Activities { get; set; } = new();

        [Range(0, double.MaxValue, ErrorMessage = "Credit limit cannot be negative.")]
        public decimal CreditLimit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Outstanding balance cannot be negative.")]
        public decimal OutstandingBalance { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var name = CollapseSpaces(Name);
            var customerCode = CollapseSpaces(CustomerCode).ToUpperInvariant();
            var email = NormalizeEmail(Email);
            var phone = NormalizePhone(Phone);
            var phoneDigits = DigitsOnly(phone);
            var status = CollapseSpaces(Status);

            if (!string.IsNullOrWhiteSpace(name))
            {
                if (!Regex.IsMatch(name, @"[A-Za-z]"))
                {
                    yield return new ValidationResult(
                        "Customer name must include letters.",
                        [nameof(Name)]);
                }

                if (!CustomerNameRegex.IsMatch(name))
                {
                    yield return new ValidationResult(
                        "Customer name can contain only letters and spaces.",
                        [nameof(Name)]);
                }
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                yield return new ValidationResult(
                    "Phone number is required.",
                    [nameof(Phone)]);
            }

            if (!string.IsNullOrWhiteSpace(customerCode) &&
                !Regex.IsMatch(customerCode, @"^[A-Z0-9-]{3,40}$"))
            {
                yield return new ValidationResult(
                    "Customer code can contain uppercase letters, numbers, and hyphens only.",
                    [nameof(CustomerCode)]);
            }

            if (!string.IsNullOrWhiteSpace(status) &&
                !new[] { "Active", "Inactive", "Blocked" }.Contains(status, StringComparer.OrdinalIgnoreCase))
            {
                yield return new ValidationResult(
                    "Select a valid customer status.",
                    [nameof(Status)]);
            }

            if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
            {
                yield return new ValidationResult(
                    "Enter a valid email address such as user@gmail.com.",
                    [nameof(Email)]);
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                if (!Regex.IsMatch(phone, @"^\+?\d+$"))
                {
                    yield return new ValidationResult(
                        "Use digits only, with an optional +91 prefix.",
                        [nameof(Phone)]);
                }

                if (phone.StartsWith("+", StringComparison.Ordinal) &&
                    !phone.StartsWith("+91", StringComparison.Ordinal))
                {
                    yield return new ValidationResult(
                        "Only +91 country code is supported for customer phone numbers.",
                        [nameof(Phone)]);
                }

                if (phone.StartsWith("+91", StringComparison.Ordinal) && phoneDigits.Length != 12)
                {
                    yield return new ValidationResult(
                        "Use +91 followed by a 10-digit mobile number.",
                        [nameof(Phone)]);
                }

                if (!phone.StartsWith("+", StringComparison.Ordinal) && phoneDigits.Length != 10)
                {
                    yield return new ValidationResult(
                        "Phone number must contain exactly 10 digits.",
                        [nameof(Phone)]);
                }

                if (HasSpamDigitPattern(phoneDigits))
                {
                    yield return new ValidationResult(
                        "Enter a real phone number, not repeated digits.",
                        [nameof(Phone)]);
                }
            }

            foreach (var field in new[] { Company, Address, City, Notes })
            {
                if (!string.IsNullOrEmpty(field) && string.IsNullOrWhiteSpace(CollapseSpaces(field)))
                {
                    yield return new ValidationResult(
                        "Text fields cannot contain only blank spaces.");
                }
            }

            var taxNumber = NormalizeGstNumber(string.IsNullOrWhiteSpace(GstNumber) ? TaxNumber : GstNumber);

            if (!string.IsNullOrWhiteSpace(taxNumber) && taxNumber.Length != 15)
            {
                yield return new ValidationResult(
                    "GST number must contain 15 characters.",
                    [nameof(GstNumber)]);
            }
            else if (!string.IsNullOrWhiteSpace(taxNumber) && !GstRegex.IsMatch(taxNumber))
            {
                yield return new ValidationResult(
                    "Enter a valid GST number.",
                    [nameof(GstNumber)]);
            }
        }

        private static bool IsValidEmail(string value)
        {
            if (value.Length > 254 || value.Contains("..", StringComparison.Ordinal))
            {
                return false;
            }

            var parts = value.Split('@');

            if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
            {
                return false;
            }

            var domainLabels = parts[1].Split('.');

            return EmailRegex.IsMatch(value) &&
                domainLabels.Length >= 2 &&
                domainLabels.All(label =>
                    label.Length > 0 &&
                    label.Length <= 63 &&
                    !label.StartsWith("-", StringComparison.Ordinal) &&
                    !label.EndsWith("-", StringComparison.Ordinal));
        }

        private static string NormalizeEmail(string? value)
            => Regex.Replace((value ?? string.Empty).Trim().ToLowerInvariant(), @"\s+", string.Empty);

        public static string CollapseSpaces(string? value)
            => Regex.Replace((value ?? string.Empty).Trim(), @"\s+", " ");

        public static string NormalizeGstNumber(string? value)
            => Regex.Replace(value ?? string.Empty, @"[^A-Za-z0-9]", string.Empty)
                .ToUpperInvariant();

        public static string NormalizePhone(string? value)
        {
            var cleanValue = Regex.Replace(value ?? string.Empty, @"[^\d+]", string.Empty).Trim();
            var hasLeadingPlus = cleanValue.StartsWith("+", StringComparison.Ordinal);
            var digits = DigitsOnly(cleanValue);

            if (digits.Length == 0)
            {
                return string.Empty;
            }

            if (!hasLeadingPlus)
            {
                return digits;
            }

            return $"+{digits}";
        }

        private static string DigitsOnly(string? value)
            => Regex.Replace(value ?? string.Empty, @"\D", string.Empty);

        private static bool HasSpamDigitPattern(string digits)
        {
            var localDigits = digits.Length == 12 && digits.StartsWith("91", StringComparison.Ordinal)
                ? digits[2..]
                : digits;

            return localDigits.Length > 0 &&
                (localDigits.All(digit => digit == localDigits[0]) ||
                 Regex.IsMatch(localDigits, @"(\d)\1{7,}"));
        }
    }

    public class CustomerContactDto
    {
        [RegularExpression(
            @"^[A-Za-z\s]+$",
            ErrorMessage = "Name can contain only letters and spaces.")]
        public string? ContactName { get; set; }
        public string? Role { get; set; }
        public string? Designation { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class CustomerAddressDto
    {
        public string? AddressType { get; set; }
        public string? AddressLine { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Pincode { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class CustomerPaymentTermDto
    {
        public int CreditDays { get; set; }
        public decimal CreditLimit { get; set; }
        public string? PaymentMode { get; set; }
        public string? Notes { get; set; }
    }

    public class CustomerBankDetailDto
    {
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? IfscCode { get; set; }
        public string? Branch { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class CustomerActivityDto
    {
        public string? ActivityType { get; set; }
        public string? Description { get; set; }
    }
}
