namespace WilvanBil.NationalRegisterNumber;

/// <summary>
/// Provides extension methods for working with Belgian National Register Numbers as strings.
/// </summary>
/// <remarks>
/// These extension methods provide formatting and data extraction capabilities for Belgian national register numbers.
/// The extraction methods deliberately skip full checksum validation for performance reasons.
/// </remarks>
public static class NationalRegisterNumberStringExtensions
{
    const int NationalRegisterNumberLength = 11;

    /// <summary>
    /// Formats a Belgian National Register Number into the official format "YY.MM.DD-XXX.CC".
    /// </summary>
    /// <param name="nationalRegisterNumber">The unformatted 11-digit national register number as a string.</param>
    /// <returns>The formatted national register number in "YY.MM.DD-XXX.CC" format, or the original string if the input length is invalid.</returns>
    /// <remarks>
    /// <para>This method does NOT validate the checksum or verify if the number is valid.</para>
    /// <para>It only performs a basic length check. Use <see cref="NationalRegisterNumberGenerator.IsValid"/> for full validation.</para>
    /// <para>If the input is null, whitespace, or not exactly 11 characters, the original string is returned unchanged.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// string unformatted = "90022742191";
    /// string formatted = unformatted.ToFormattedNationalRegisterNumber();
    /// // Returns: "90.02.27-421.91"
    /// </code>
    /// </example>
    public static string ToFormattedNationalRegisterNumber(this string nationalRegisterNumber)
    {
        // Validate the input (basic length check)
        if (string.IsNullOrWhiteSpace(nationalRegisterNumber) || nationalRegisterNumber.Length != NationalRegisterNumberLength)
            return nationalRegisterNumber;

        // Extract parts of the national register number using range operators
        var birthDate = nationalRegisterNumber[..6];     // YYMMDD
        var followNumber = nationalRegisterNumber[6..9]; // XXX
        var controlNumber = nationalRegisterNumber[9..11]; // CC

        // Format into "YY.MM.DD-XXX.CC"
        return $"{birthDate[..2]}.{birthDate[2..4]}.{birthDate[4..6]}-{followNumber}.{controlNumber}";
    }

    /// <summary>
    /// Attempts to extract the biological sex from a Belgian National Register Number based on the follow number's last digit.
    /// </summary>
    /// <param name="nationalRegisterNumber">The 11-digit national register number.</param>
    /// <param name="sex">When this method returns, contains the extracted biological sex if successful; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the biological sex was successfully extracted; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// <para>This method extracts sex based on the last digit of the follow number (position 8 in the full number):</para>
    /// <list type="bullet">
    /// <item><description>Even digit (0, 2, 4, 6, 8) = Female</description></item>
    /// <item><description>Odd digit (1, 3, 5, 7, 9) = Male</description></item>
    /// </list>
    /// <para>This method does NOT validate the full checksum for performance reasons.</para>
    /// <para>It only checks that the input is 11 digits long and contains only numeric characters.</para>
    /// <para>Use <see cref="NationalRegisterNumberGenerator.IsValid"/> first if you need full validation.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// string number = "90022742191";
    /// if (number.TryExtractBiologicalSex(out var sex))
    /// {
    ///     Console.WriteLine($"Sex: {sex}"); // Output: "Sex: Male" (digit at position 8 is '1', odd)
    /// }
    /// </code>
    /// </example>
    public static bool TryExtractBiologicalSex(this string nationalRegisterNumber, out BiologicalSex? sex)
    {
        sex = null;

        // Validate input: Must be 11 digits long
        if (string.IsNullOrWhiteSpace(nationalRegisterNumber) || nationalRegisterNumber.Length != NationalRegisterNumberLength)
            return false;

        // Ensure all characters are digits
        if (!nationalRegisterNumber.All(char.IsDigit))
            return false;

        // Extract the last digit of the follow number (position 8 in the full number)
        // The follow number is XXX (positions 6-8), and the sex is determined by the last digit
        char lastFollowDigit = nationalRegisterNumber[8];
        int digit = lastFollowDigit - '0';

        // Determine biological sex: even = female, odd = male
        sex = digit % 2 == 0 ? BiologicalSex.Female : BiologicalSex.Male;
        return true;
    }

    /// <summary>
    /// Attempts to extract the birth date from a Belgian National Register Number.
    /// </summary>
    /// <param name="nationalRegisterNumber">The 11-digit national register number.</param>
    /// <param name="birthDate">When this method returns, contains the extracted birth date if successful; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the birth date was successfully extracted; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// <para>This method extracts the birth date and automatically determines the correct century (1900s vs 2000s) by:</para>
    /// <list type="number">
    /// <item><description>Parsing the date from the first 6 digits (YYMMDD)</description></item>
    /// <item><description>Checking the checksum to determine if the birth year is before or after 2000</description></item>
    /// </list>
    /// <para>This method does NOT perform full validation of the national register number.</para>
    /// <para>It only checks basic format and uses the checksum to infer the century.</para>
    /// <para>Use <see cref="NationalRegisterNumberGenerator.IsValid"/> first if you need full validation.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// string number = "90022742191";
    /// if (number.TryExtractBirthDate(out var birthDate))
    /// {
    ///     Console.WriteLine($"Birth Date: {birthDate}"); // Output: "Birth Date: 1990-02-27"
    /// }
    /// 
    /// string y2kNumber = "00010100121";
    /// if (y2kNumber.TryExtractBirthDate(out var y2kBirthDate))
    /// {
    ///     Console.WriteLine($"Birth Date: {y2kBirthDate}"); // Output: "Birth Date: 2000-01-01"
    /// }
    /// </code>
    /// </example>
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

