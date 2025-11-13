namespace WilvanBil.NationalRegisterNumber;

/// <summary>
/// Represents the biological sex of an individual as recorded in the Belgian National Register.
/// </summary>
/// <remarks>
/// In the Belgian National Register number format, biological sex is encoded in the last digit of the follow number (XXX):
/// <list type="bullet">
/// <item><description>Even last digit (0, 2, 4, 6, 8) indicates Female</description></item>
/// <item><description>Odd last digit (1, 3, 5, 7, 9) indicates Male</description></item>
/// </list>
/// This encoding is part of the official Belgian government specification.
/// </remarks>
public enum BiologicalSex
{
    /// <summary>
    /// Biological female. Represented by an even last digit in the follow number.
    /// </summary>
    Female,

    /// <summary>
    /// Biological male. Represented by an odd last digit in the follow number.
    /// </summary>
    Male
}
