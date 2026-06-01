using System;
using System.Text.RegularExpressions;

namespace LUnivers_Beaute.Helpers
{
    public static class ValidationHelper
    {
        private static readonly Regex PhoneRegex = new Regex(@"^(0[35789])[0-9]{8}$");
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        private static readonly Regex NumberRegex = new Regex(@"^[0-9]+$");

        /// <summary>
        /// Validates that a string is a valid Vietnamese phone number.
        /// Vietnamese mobile phone numbers start with 03, 05, 07, 08, 09 and have exactly 10 digits.
        /// </summary>
        public static bool IsPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return PhoneRegex.IsMatch(phone.Trim());
        }

        /// <summary>
        /// Validates that a string is a valid email address.
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return EmailRegex.IsMatch(email.Trim());
        }

        /// <summary>
        /// Validates that a string contains only digits.
        /// </summary>
        public static bool IsNumeric(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            return NumberRegex.IsMatch(text.Trim());
        }

        /// <summary>
        /// Validates that a string is a positive integer.
        /// </summary>
        public static bool IsPositiveInteger(string text, out int val)
        {
            val = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            return int.TryParse(text.Trim(), out val) && val > 0;
        }

        /// <summary>
        /// Validates that a string is a non-negative integer (can be zero).
        /// </summary>
        public static bool IsNonNegativeInteger(string text, out int val)
        {
            val = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            return int.TryParse(text.Trim(), out val) && val >= 0;
        }

        /// <summary>
        /// Validates that a string is a positive decimal.
        /// </summary>
        public static bool IsPositiveDecimal(string text, out decimal val)
        {
            val = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            return decimal.TryParse(text.Trim(), out val) && val > 0;
        }

        /// <summary>
        /// Validates that a string is a non-negative decimal (can be zero).
        /// </summary>
        public static bool IsNonNegativeDecimal(string text, out decimal val)
        {
            val = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            return decimal.TryParse(text.Trim(), out val) && val >= 0;
        }

        /// <summary>
        /// Validates that a name is valid (no digits or weird characters).
        /// </summary>
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            string clean = name.Trim();
            if (clean.Length < 2) return false;
            // Names shouldn't contain digits or common symbols
            foreach (char c in clean)
            {
                if (char.IsDigit(c) || char.IsSymbol(c) || "@#$%^&*()_+={}[]|\\:;\"'<>,.?/".Contains(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
