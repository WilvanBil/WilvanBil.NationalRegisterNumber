using FluentAssertions;
using FsCheck.Xunit;
using Xunit;

namespace WilvanBil.NationalRegisterNumber.UnitTests;

public class NationalRegisterNumberValidationTests
{
    [Theory]
    [InlineData("90022742191")]
    public void ValidateShouldReturnTrue(string nationalRegisterNumber)
    {
        // Act
        var result = NationalRegisterNumberGenerator.IsValid(nationalRegisterNumber);

        // Assert
        result.Should().BeTrue();
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
        result.Should().BeFalse();
    }

    /// <summary>
    /// Ensures the NationalRegisterNumberGenerator.Generate method produces valid results for any DateTime input.
    /// FsCheck generates a wide range of random and edge-case DateTime values for testing.
    /// </summary>
    /// <param name="birthDate">The randomly generated DateTime input.</param>
    [Property(Verbose = true)]
    public void GenerateShouldWorkForAnyDateTime(DateTime birthDate)
    {
        // Act
        var nationalRegisterNumber = NationalRegisterNumberGenerator.Generate(DateOnly.FromDateTime(birthDate));

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(nationalRegisterNumber));
    }
}
