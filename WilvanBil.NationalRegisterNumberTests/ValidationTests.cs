using FsCheck.Xunit;
using Xunit;

namespace WilvanBil.NationalRegisterNumber.UnitTests;

public class ValidationTests
{
    [Theory]
    [InlineData("90022742191")]
    public void ValidateShouldReturnTrue(string nationalRegisterNumber)
    {
        // Act
        var result = NationalRegisterNumberGenerator.IsValid(nationalRegisterNumber);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("90.02.27-421.91")]
    [InlineData("80.05.26-004.58")]
    [InlineData("82.06.18-789.47")]
    public void ValidateShouldReturnTrue_FormattedInput(string nationalRegisterNumber)
    {
        // Act
        var result = NationalRegisterNumberGenerator.IsValid(nationalRegisterNumber);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("12345678910")]
    [InlineData("12345621748")]
    [InlineData("12345621777")]
    [InlineData("Test")]
    [InlineData("00000000000")]
    [InlineData("99999999999")]
    [InlineData("!@#!@%^@^@$^&@$^@sdfasdf")]
    [InlineData("$^@#^@##$44")]
    [InlineData("15435#$%4354dfsg")]
    [InlineData("90022742192")]
    public void ValidateShouldReturnFalse(string nationalRegisterNumber)
    {
        // Act
        var result = NationalRegisterNumberGenerator.IsValid(nationalRegisterNumber);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Ensures the NationalRegisterNumberGenerator.Generate method produces valid results for any valid DateTime input.
    /// FsCheck generates a wide range of random and edge-case DateTime values for testing.
    /// Dates before 1900 are skipped as they are not supported by the Belgian National Register system.
    /// </summary>
    /// <param name="birthDate">The randomly generated DateTime input.</param>
    [Property(Verbose = true)]
    public void GenerateShouldWorkForAnyValidDateTime(DateTime birthDate)
    {
        // Skip dates before 1900 (not supported)
        if (birthDate.Year < 1900)
            return;

        // Act
        var nationalRegisterNumber = NationalRegisterNumberGenerator.Generate(DateOnly.FromDateTime(birthDate));

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(nationalRegisterNumber));
    }

    /// <summary>
    /// Verifies that generating with dates before 1900 throws ArgumentException.
    /// </summary>
    [Theory]
    [InlineData(1899, 12, 31)]
    [InlineData(1850, 6, 15)]
    [InlineData(1800, 1, 1)]
    [InlineData(1500, 3, 20)]
    public void GenerateWithDateBefore1900_ShouldThrowArgumentException(int year, int month, int day)
    {
        // Arrange
        var invalidDate = new DateOnly(year, month, day);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => NationalRegisterNumberGenerator.Generate(invalidDate));
        Assert.Contains("Birthdate can't be before", exception.Message);
    }
}
