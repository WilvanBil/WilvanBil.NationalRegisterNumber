using Xunit;

namespace WilvanBil.NationalRegisterNumber.UnitTests;

public class FormattingTests
{
    [Theory]
    [InlineData("90022742191", "90.02.27-421.91")]
    [InlineData("12345678910", "12.34.56-789.10")]
    [InlineData("00000000000", "00.00.00-000.00")]
    [InlineData("", "")]
    [InlineData("invalid", "invalid")]
    public void ToFormattedNationalRegisterNumber_ShouldFormatCorrectly(string input, string expected)
    {
        // Act
        var result = input.ToFormattedNationalRegisterNumber();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("          ")]
    public void ToFormattedNationalRegisterNumber_WithNullOrWhitespace_ReturnsOriginal(string? input)
    {
        // Act
        var result = input!.ToFormattedNationalRegisterNumber();

        // Assert
        Assert.Equal(input, result);
    }

    [Theory]
    [InlineData("12345")] // Too short
    [InlineData("123456789012")] // Too long
    [InlineData("1234567890")] // 10 chars
    [InlineData("123456789012345")] // 15 chars
    public void ToFormattedNationalRegisterNumber_WithInvalidLength_ReturnsOriginal(string input)
    {
        // Act
        var result = input.ToFormattedNationalRegisterNumber();

        // Assert
        Assert.Equal(input, result);
    }
}
