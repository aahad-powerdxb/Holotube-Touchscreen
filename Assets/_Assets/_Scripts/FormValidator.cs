using System.Text.RegularExpressions;

public static class FormValidator
{
    // Regex for Email (Standard Pattern)
    // Matches: something@something.domain
    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.IgnoreCase
    );

    // Regex for Phone
    // Matches: Optional '+' followed by 7 to 15 digits. Allows spaces or dashes.
    // Examples: +971501234567, 0501234567, 123-456-7890
    private static readonly Regex PhoneRegex = new Regex(
        @"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$"
    );

    public static bool IsNameValid(string name)
    {
        // Name must not be empty and at least 2 characters
        return !string.IsNullOrWhiteSpace(name) && name.Trim().Length >= 2;
    }

    public static bool IsEmailValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        return EmailRegex.IsMatch(email);
    }

    public static bool IsPhoneValid(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return false;
        return PhoneRegex.IsMatch(phone);
    }
}