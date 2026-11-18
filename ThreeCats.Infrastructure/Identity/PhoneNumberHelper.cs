namespace ThreeCats.Infrastructure.Identity;

public static class PhoneNumberHelper
{
    public static string NormalizeForUserName(string phoneNumber)
    {
        var digits = new string(
            phoneNumber
                .Where(char.IsDigit)
                .ToArray());

        if (digits.StartsWith('8'))
            digits = "7" + digits[1..];
        else if (digits.Length == 10)
            digits = "7" + digits;

        return digits;
    }
}
