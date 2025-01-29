using FluentAssertions;
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
        result.Should().Be(expected);
    }
}
