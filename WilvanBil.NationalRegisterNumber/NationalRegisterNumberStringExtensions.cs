
namespace WilvanBil.NationalRegisterNumber;

public static class NationalRegisterNumberStringExtensions
{
    /// <summary>
    /// Formats a valid Belgian National Register Number into the format "YY.MM.DD-XXX.CC".
    /// It does not check if it's valid!
    /// </summary>
    /// <param name="nationalRegisterNumber">The national register number as a string.</param>
    /// <returns>The formatted national register number, or the original string if invalid.</returns>
    public static string ToFormattedNationalRegisterNumber(this string nationalRegisterNumber)
    {
        const int nationalRegisterNumberLength = 11;

        // Validate the input (basic length check)
        if (string.IsNullOrWhiteSpace(nationalRegisterNumber) || nationalRegisterNumber.Length != nationalRegisterNumberLength)
            return nationalRegisterNumber;

        // Extract parts of the national register number
        var birthDate = nationalRegisterNumber[..6]; // YYMMDD
        var followNumber = nationalRegisterNumber.Substring(6, 3); // XXX
        var controlNumber = nationalRegisterNumber.Substring(9, 2); // CC

        // Format into "YY.MM.DD-XXX.CC"
        return $"{birthDate[..2]}.{birthDate.Substring(2, 2)}.{birthDate.Substring(4, 2)}-{followNumber}.{controlNumber}";
    }
}

