
namespace WilvanBil.NationalRegisterNumber;

/// <summary>
/// Provides extension methods for working with Belgian National Register Numbers as strings.
/// </summary>
public static class NationalRegisterNumberStringExtensions
{
    const int NationalRegisterNumberLength = 11;

    /// <summary>
    /// Formats a valid Belgian National Register Number into the format "YY.MM.DD-XXX.CC".
    /// It does not check if it's valid!
    /// </summary>
    /// <param name="nationalRegisterNumber">The national register number as a string.</param>
    /// <returns>The formatted national register number, or the original string if invalid.</returns>
    public static string ToFormattedNationalRegisterNumber(this string nationalRegisterNumber)
    {

        // Validate the input (basic length check)
        if (string.IsNullOrWhiteSpace(nationalRegisterNumber) || nationalRegisterNumber.Length != NationalRegisterNumberLength)
            return nationalRegisterNumber;

        // Extract parts of the national register number
        var birthDate = nationalRegisterNumber[..6]; // YYMMDD
        var followNumber = nationalRegisterNumber.Substring(6, 3); // XXX
        var controlNumber = nationalRegisterNumber.Substring(9, 2); // CC

        // Format into "YY.MM.DD-XXX.CC"
        return $"{birthDate[..2]}.{birthDate.Substring(2, 2)}.{birthDate.Substring(4, 2)}-{followNumber}.{controlNumber}";
    }

    /// <summary>
    /// Attempts to extract the biological sex from a Belgian National Register Number without checking full validity
    /// </summary>
    /// <param name="nationalRegisterNumber">The 11-digit national register number.</param>
    /// <param name="sex">The extracted biological sex if successful, otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the biological sex was successfully extracted; otherwise, <see langword="false"/>.</returns>
    public static bool TryExtractBiologicalSex(this string nationalRegisterNumber, out BiologicalSex? sex)
    {
        sex = null;

        // Validate input: Must be 11 digits long
        if (string.IsNullOrWhiteSpace(nationalRegisterNumber) || nationalRegisterNumber.Length != NationalRegisterNumberLength)
            return false;

        // Ensure all characters are digits
        if (!nationalRegisterNumber.All(char.IsDigit))
            return false;

        // Extract follow number (XXX part) directly as an integer
        if (!int.TryParse(nationalRegisterNumber.AsSpan(6, 3), out int followNumber))
            return false;

        // Determine biological sex
        sex = followNumber % 2 == 0 ? BiologicalSex.Female : BiologicalSex.Male;
        return true;
    }

    /// <summary>
    /// Attempts to extract the birth date from a Belgian National Register Number.
    /// </summary>
    /// <param name="nationalRegisterNumber">The 11-digit national register number.</param>
    /// <param name="birthDate">The extracted birth date if successful, otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the birth date was successfully extracted; otherwise, <see langword="false"/>.</returns>
    public static bool TryExtractBirthDate(this string nationalRegisterNumber, out DateOnly? birthDate)
    {
        birthDate = null;

        // Basic validation: Length check
        if (string.IsNullOrWhiteSpace(nationalRegisterNumber) || nationalRegisterNumber.Length != NationalRegisterNumberLength)
            return false;

        // Extract birth date part (YYMMDD)
        string birthDatePart = nationalRegisterNumber[..6];

        // Validate and parse birthdate
        if (!DateOnly.TryParseExact(birthDatePart, "yyMMdd", out var parsedDate))
            return false;

        // Adjust year if necessary (handling pre-2000 vs post-2000 birth years)
        int year = parsedDate.Year;
        if (year >= 0 && year <= 99)
        {
            long dividend = long.Parse(nationalRegisterNumber[..9]); // First 9 digits
            long remainder = dividend % 97;
            long controlNumber = 97 - remainder;

            if (controlNumber.ToString("00") != nationalRegisterNumber[9..])
            {
                // Try assuming the person was born after 1999
                year += 2000;
            }
            else
            {
                year += 1900;
            }

            parsedDate = new DateOnly(year, parsedDate.Month, parsedDate.Day);
        }

        birthDate = parsedDate;
        return true;
    }
}

