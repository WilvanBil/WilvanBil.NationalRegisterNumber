using System.Globalization;

namespace WilvanBil.NationalRegisterNumber;

public static class NationalRegisterNumberGenerator
{
    private const int Divisor = 97;
    private const int FollowNumberMin = 1;
    private const int FollowNumberMax = 998;

    private static readonly DateTime AbsoluteMinDate = new(1900, 1, 1);
    private static readonly Random Randomizer = new();

    /// <summary>
    /// Validates strings if it's a valid national register number for Belgium.
    /// </summary>
    /// <param name="nationalRegisterNumber"></param>
    /// <returns>true if it's valid, otherwise false</returns>
    public static bool IsValid(string nationalRegisterNumber)
    {
        // Sanitize input
        if (string.IsNullOrEmpty(nationalRegisterNumber))
            return false;

        // Filter input
        string numbersOnly = new(nationalRegisterNumber.Trim().Where(x => char.IsDigit(x)).ToArray());

        // Check null and length
        if (string.IsNullOrEmpty(numbersOnly) || numbersOnly.Length != 11)
            return false;

        // check date

        if (!DateTime.TryParseExact(
         numbersOnly[..6],
         "yyMMdd",
         CultureInfo.InvariantCulture,
         DateTimeStyles.None,
         out var d))
            return false;

        // Check control number
        if (!long.TryParse(numbersOnly[..9], out var dividend))
            return false;

        var remainder = dividend % Divisor;
        var controlNumber = Divisor - remainder;

        if (!long.TryParse(numbersOnly[9..], out var actualControlNumber))
            return false;

        // Born before 2000/01/01
        if (controlNumber == actualControlNumber)
            return true;

        if (!long.TryParse($"2{numbersOnly[..9]}", out dividend))
            return false;

        remainder = dividend % Divisor;
        controlNumber = Divisor - remainder;

        // Born after 1999/12/31
        return controlNumber == actualControlNumber;
    }

    public static string Generate(DateTime birthDate, int followNumber)
    {
        // Sanitize
        if (birthDate < AbsoluteMinDate)
            throw new ArgumentException($"Birthdate can't be before {AbsoluteMinDate.ToShortDateString()}", nameof(birthDate));

        if (birthDate > DateTime.Today)
            throw new ArgumentException($"Date of birth should be before today {DateTime.Today.ToShortDateString()}", nameof(birthDate));

        if (followNumber < FollowNumberMin || followNumber > FollowNumberMax)
            throw new ArgumentException("Follow number should be (inclusive) between 1 and 998", nameof(followNumber));

        // Calculate control number
        var birthDatePart = birthDate.ToString("yyMMdd");
        var followNumberPart = followNumber.ToString().PadLeft(3, '0');

        long dividend;
        if (birthDate.Year > 1999)
            dividend = long.Parse($"2{birthDatePart}{followNumberPart}");
        else
            dividend = long.Parse($"{birthDatePart}{followNumberPart}");

        var remainder = dividend % Divisor;
        var controlNumber = Divisor - remainder;
        var controlNumberPart = controlNumber.ToString().PadLeft(2, '0');

        return $"{birthDatePart}{followNumberPart}{controlNumberPart}";
    }

    public static string Generate() => Generate(GenerateBirthDate(), GenerateFollowNumber());
    public static string Generate(DateTime birthDate) => Generate(birthDate, GenerateFollowNumber());
    public static string Generate(BiologicalSex sex) => Generate(GenerateBirthDate(), GenerateFollowNumber(sex));
    public static string Generate(DateTime birthDate, BiologicalSex sex) => Generate(birthDate, GenerateFollowNumber(sex));
    public static string Generate(DateTime minDate, DateTime maxDate) => Generate(GenerateBirthDate(minDate, maxDate));
    public static string Generate(DateTime minDate, DateTime maxDate, BiologicalSex sex) => Generate(GenerateBirthDate(minDate, maxDate), sex);

    private static int GenerateFollowNumber(BiologicalSex sex)
    {
        var followNumber = GenerateFollowNumber();
        if (sex == BiologicalSex.Female)
        {
            if (followNumber % 2 != 0)
                followNumber++;
        }
        else
        {
            if (followNumber % 2 == 0)
                followNumber--;
        }

        return followNumber;
    }
    private static int GenerateFollowNumber() => Randomizer.Next(FollowNumberMin, FollowNumberMax);
    private static DateTime GenerateBirthDate() => GenerateBirthDate(AbsoluteMinDate, DateTime.Today);
    private static DateTime GenerateBirthDate(DateTime minDate, DateTime maxDate)
    {
        if (minDate > maxDate)
            throw new ArgumentException($"Minimum date {minDate} can't be after maximum date {maxDate}");

        var range = (maxDate.Date - minDate.Date).Days;
        return minDate.Date.AddDays(Randomizer.Next(range));
    }
}
