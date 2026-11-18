using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ThreeCats.Application.Validation;

public static class PhoneNumberValidator
{
    private static readonly Regex PhoneRegex = new(
        @"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}$",
        RegexOptions.Compiled);

    public static void Validate(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ValidationException(
                "Введите номер телефона.");
        }

        if (!PhoneRegex.IsMatch(phoneNumber))
        {
            throw new ValidationException(
                "Введите полный номер телефона в формате +7 (999) 999-99-99.");
        }
    }
}
