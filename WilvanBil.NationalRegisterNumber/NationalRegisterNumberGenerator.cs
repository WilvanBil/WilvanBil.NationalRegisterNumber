namespace WilvanBil.NationalRegisterNumber;

/// <summary>
/// Provides methods to generate and validate Belgian national register numbers.
/// </summary>
/// <remarks>
/// Belgian national register numbers follow the format YY.MM.DD-XXX.CC where:
/// <list type="bullet">
/// <item><description>YYMMDD: Birth date (2-digit year, month, day)</description></item>
/// <item><description>XXX: Follow number (001-998), with odd=male, even=female based on last digit</description></item>
/// <item><description>CC: Checksum (97 - (first 9 digits % 97)), with special handling for births after 1999</description></item>
/// </list>
/// </remarks>
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
    /// <param name="nationalRegisterNumber">The national register number to validate. Can be formatted (YY.MM.DD-XXX.CC) or unformatted (11 digits).</param>
    /// <returns><c>true</c> if the number is valid; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// This method validates the checksum according to the official Belgian specification.
    /// For births after 1999, the checksum calculation prepends '2' to the first 9 digits.
    /// The method automatically filters out non-digit characters, so formatted strings are accepted.
    /// </remarks>
    /// <example>
    /// <code>
    /// bool isValid = NationalRegisterNumberGenerator.IsValid("90022742191");
    /// // isValid = true
    /// 
    /// bool isFormatted = NationalRegisterNumberGenerator.IsValid("90.02.27-421.91");
    /// // isFormatted = true (same number, formatted)
    /// </code>
    /// </example>
    public static bool IsValid(string nationalRegisterNumber)
    {
        // Filter input
        string numbersOnly = new(nationalRegisterNumber.Where(x => char.IsDigit(x)).ToArray());

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
    /// <param name="birthDate">The birth date of the individual. Must be on or after 1900-01-01.</param>
    /// <param name="followNumber">The follow number (1-998) for the individual. Odd numbers indicate male, even numbers indicate female.</param>
    /// <returns>The generated 11-digit national register number as an unformatted string.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the birth date is before 1900-01-01 or the follow number is not between 1 and 998 (inclusive).
    /// </exception>
    /// <remarks>
    /// The checksum is automatically calculated based on the birth date and follow number.
    /// For births after 1999, the checksum calculation prepends '2' to ensure correct validation.
    /// </remarks>
    /// <example>
    /// <code>
    /// var birthDate = new DateOnly(1990, 2, 27);
    /// string number = NationalRegisterNumberGenerator.Generate(birthDate, 421);
    /// // Returns: "90022742191"
    /// </code>
    /// </example>
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
    /// Generates a random valid Belgian national register number with a random birth date and follow number.
    /// </summary>
    /// <returns>The generated 11-digit national register number.</returns>
    /// <remarks>
    /// The birth date will be randomly selected between 1900-01-01 and today's date.
    /// The follow number and biological sex will be randomly determined.
    /// </remarks>
    public static string Generate() => Generate(GenerateBirthDate(), GenerateFollowNumber());

    /// <summary>
    /// Generates a Belgian national register number for a given birth date with a random follow number.
    /// </summary>
    /// <param name="birthDate">The birth date of the individual. Must be on or after 1900-01-01.</param>
    /// <returns>The generated 11-digit national register number.</returns>
    /// <exception cref="ArgumentException">Thrown if the birth date is before 1900-01-01.</exception>
    public static string Generate(DateOnly birthDate) => Generate(birthDate, GenerateFollowNumber());

    /// <summary>
    /// Generates a Belgian national register number for a random birth date and a follow number based on the given biological sex.
    /// </summary>
    /// <param name="sex">The biological sex of the individual (determines if follow number is odd or even).</param>
    /// <returns>The generated 11-digit national register number.</returns>
    /// <remarks>
    /// Male: Follow number will have an odd last digit.
    /// Female: Follow number will have an even last digit.
    /// </remarks>
    public static string Generate(BiologicalSex sex) => Generate(GenerateBirthDate(), GenerateFollowNumber(sex));

    /// <summary>
    /// Generates a Belgian national register number for a given birth date and a follow number based on the given biological sex.
    /// </summary>
    /// <param name="birthDate">The birth date of the individual. Must be on or after 1900-01-01.</param>
    /// <param name="sex">The biological sex of the individual (determines if follow number is odd or even).</param>
    /// <returns>The generated 11-digit national register number.</returns>
    /// <exception cref="ArgumentException">Thrown if the birth date is before 1900-01-01.</exception>
    /// <remarks>
    /// Male: Follow number will have an odd last digit.
    /// Female: Follow number will have an even last digit.
    /// </remarks>
    public static string Generate(DateOnly birthDate, BiologicalSex sex) => Generate(birthDate, GenerateFollowNumber(sex));

    /// <summary>
    /// Generates a Belgian national register number for a random birth date within the specified date range.
    /// </summary>
    /// <param name="minDate">The minimum birth date in the range (inclusive).</param>
    /// <param name="maxDate">The maximum birth date in the range (inclusive).</param>
    /// <returns>The generated 11-digit national register number.</returns>
    /// <exception cref="ArgumentException">Thrown if minDate is after maxDate.</exception>
    public static string Generate(DateOnly minDate, DateOnly maxDate) => Generate(GenerateBirthDate(minDate, maxDate));

    /// <summary>
    /// Generates a Belgian national register number for a random birth date within the specified date range and a follow number based on the given biological sex.
    /// </summary>
    /// <param name="minDate">The minimum birth date in the range (inclusive).</param>
    /// <param name="maxDate">The maximum birth date in the range (inclusive).</param>
    /// <param name="sex">The biological sex of the individual (determines if follow number is odd or even).</param>
    /// <returns>The generated 11-digit national register number.</returns>
    /// <exception cref="ArgumentException">Thrown if minDate is after maxDate.</exception>
    /// <remarks>
    /// Male: Follow number will have an odd last digit.
    /// Female: Follow number will have an even last digit.
    /// </remarks>
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
