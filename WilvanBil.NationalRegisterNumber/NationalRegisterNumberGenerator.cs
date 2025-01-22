namespace WilvanBil.NationalRegisterNumber;

/// <summary>
/// Provides methods to generate and validate Belgian national register numbers.
/// </summary>
public static class NationalRegisterNumberGenerator
{
    private const int NationalRegisterNumberLength = 11;
    private const int BirthDateLength = 6;
    private const int BirthDateWithControlNumberLength = 9;
    private const int Divisor = 97;
    private const int FollowNumberMin = 1;
    private const int FollowNumberMax = 998;

    private static readonly DateOnly _absoluteMinDate = new(1900, 1, 1);
    private static readonly Random _randomizer = Random.Shared;
    private static readonly TimeProvider _timeProvider = TimeProvider.System;

    /// <summary>
    /// Validates whether a given string is a valid Belgian national register number.
    /// </summary>
    /// <param name="nationalRegisterNumber">The national register number to validate.</param>
    /// <returns><c>true</c> if the number is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValid(string nationalRegisterNumber)
    {
        // Sanitize input
        if (string.IsNullOrEmpty(nationalRegisterNumber))
            return false;

        // Filter input
        string numbersOnly = new(nationalRegisterNumber.Trim().Where(x => char.IsDigit(x)).ToArray());

        // Check null and length
        if (string.IsNullOrEmpty(numbersOnly) || numbersOnly.Length != NationalRegisterNumberLength)
            return false;

        var possibleBirthDate = numbersOnly[..BirthDateLength];
        if (!DateOnly.TryParseExact(numbersOnly[..BirthDateLength], "yyMMdd", out var birthDate))
            return false;

        // Check control number
        if (!long.TryParse(numbersOnly[..BirthDateWithControlNumberLength], out var dividend))
            return false;

        var remainder = dividend % Divisor;
        var controlNumber = Divisor - remainder;

        if (!long.TryParse(numbersOnly[BirthDateWithControlNumberLength..], out var actualControlNumber))
            return false;

        // Born before 2000/01/01
        if (controlNumber == actualControlNumber)
            return true;

        if (!long.TryParse($"2{numbersOnly[..BirthDateWithControlNumberLength]}", out dividend))
            return false;

        remainder = dividend % Divisor;
        controlNumber = Divisor - remainder;

        // Born after 1999/12/31
        return controlNumber == actualControlNumber;
    }

    /// <summary>
    /// Generates a Belgian national register number based on a given birth date and follow number.
    /// </summary>
    /// <param name="birthDate">The birth date of the individual.</param>
    /// <param name="followNumber">The follow number (1-998) for the individual.</param>
    /// <returns>The generated national register number.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the birth date is before the minimum allowed date or the follow number is out of range.
    /// </exception>
    public static string Generate(DateOnly birthDate, int followNumber)
    {
        // Sanitize
        if (birthDate < _absoluteMinDate)
            throw new ArgumentException($"Birthdate can't be before {_absoluteMinDate.ToShortDateString()}", nameof(birthDate));

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

    /// <summary>
    /// Generates a random valid Belgian national register number.
    /// </summary>
    /// <returns>The generated national register number.</returns>
    public static string Generate() => Generate(GenerateBirthDate(), GenerateFollowNumber());

    /// <summary>
    /// Generates a Belgian national register number for a given birth date with a random follow number.
    /// </summary>
    /// <param name="birthDate">The birth date of the individual.</param>
    /// <returns>The generated national register number.</returns>
    public static string Generate(DateOnly birthDate) => Generate(birthDate, GenerateFollowNumber());

    /// <summary>
    /// Generates a Belgian national register number for a random birth date and a follow number based on the given biological sex.
    /// </summary>
    /// <param name="sex">The biological sex of the individual.</param>
    /// <returns>The generated national register number.</returns>
    public static string Generate(BiologicalSex sex) => Generate(GenerateBirthDate(), GenerateFollowNumber(sex));

    /// <summary>
    /// Generates a Belgian national register number for a given birth date and a follow number based on the given biological sex.
    /// </summary>
    /// <param name="birthDate">The birth date of the individual.</param>
    /// <param name="sex">The biological sex of the individual.</param>
    /// <returns>The generated national register number.</returns>
    public static string Generate(DateOnly birthDate, BiologicalSex sex) => Generate(birthDate, GenerateFollowNumber(sex));

    /// <summary>
    /// Generates a Belgian national register number for a random birth date within the specified date range.
    /// </summary>
    /// <param name="minDate">The minimum birth date in the range.</param>
    /// <param name="maxDate">The maximum birth date in the range.</param>
    /// <returns>The generated national register number.</returns>
    public static string Generate(DateOnly minDate, DateOnly maxDate) => Generate(GenerateBirthDate(minDate, maxDate));

    /// <summary>
    /// Generates a Belgian national register number for a random birth date within the specified date range and a follow number based on the given biological sex.
    /// </summary>
    /// <param name="minDate">The minimum birth date in the range.</param>
    /// <param name="maxDate">The maximum birth date in the range.</param>
    /// <param name="sex">The biological sex of the individual.</param>
    /// <returns>The generated national register number.</returns>
    public static string Generate(DateOnly minDate, DateOnly maxDate, BiologicalSex sex) => Generate(GenerateBirthDate(minDate, maxDate), sex);

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
    private static int GenerateFollowNumber() => _randomizer.Next(FollowNumberMin, FollowNumberMax);
    private static DateOnly GenerateBirthDate() => GenerateBirthDate(_absoluteMinDate, DateOnly.FromDateTime(_timeProvider.GetUtcNow().Date));
    private static DateOnly GenerateBirthDate(DateOnly minDate, DateOnly maxDate)
    {
        if (minDate > maxDate)
            throw new ArgumentException($"Minimum date {minDate} can't be after maximum date {maxDate}");

        var range = (maxDate.ToDateTime(TimeOnly.MinValue) - minDate.ToDateTime(TimeOnly.MinValue)).Days;
        return minDate.AddDays(_randomizer.Next(range));
    }
}
