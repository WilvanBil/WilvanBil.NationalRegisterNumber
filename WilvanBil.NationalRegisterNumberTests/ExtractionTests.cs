using Xunit;

namespace WilvanBil.NationalRegisterNumber.UnitTests;

public class ExtractionTests
{
    [Theory]
    [InlineData("90022742191", "1990-02-27")]
    [InlineData("00010100121", "2000-01-01")]
    [InlineData("99010199991", "1999-01-01")]
    [InlineData("05010156780", "2005-01-01")]
    public void TryExtractBirthDate_ValidNumbers_ReturnsCorrectDate(string nationalRegisterNumber, string expectedDate)
    {
        // Arrange
        var expectedBirthDate = DateOnly.Parse(expectedDate);

        // Act
        bool result = nationalRegisterNumber.TryExtractBirthDate(out var birthDate);

        // Assert
        Assert.True(result);
        Assert.Equal(expectedBirthDate, birthDate);
    }

    [Theory]
    [InlineData("12345678910")] // Invalid format
    [InlineData("00000000000")] // Clearly invalid
    [InlineData("99999999999")] // Invalid control number
    [InlineData("abcdefghijk")] // Non-numeric
    [InlineData("")]            // Empty
    public void TryExtractBirthDate_InvalidNumbers_ReturnsFalse(string invalidNationalRegisterNumber)
    {
        // Act
        bool result = invalidNationalRegisterNumber.TryExtractBirthDate(out var birthDate);

        // Assert
        Assert.False(result);
        Assert.Null(birthDate);
    }

    [Theory]
    [InlineData("90022742191", BiologicalSex.Male)]  // Born 1990-02-27, Male
    [InlineData("80052600458", BiologicalSex.Female)] // Born 1980-05-26, Female
    [InlineData("03071512331", BiologicalSex.Male)]  // Born 2003-07-15, Male
    [InlineData("95041224690", BiologicalSex.Female)] // Born 1995-04-12, Female
    [InlineData("82061878947", BiologicalSex.Male)]  // Born 1982-06-18, Male
    [InlineData("01010135624", BiologicalSex.Female)] // Born 2001-01-01, Female
    public void TryExtractBiologicalSex_ValidNumbers_ReturnsCorrectSex(string nationalRegisterNumber, BiologicalSex expectedSex)
    {
        // Act
        bool result = nationalRegisterNumber.TryExtractBiologicalSex(out var sex);

        // Assert
        Assert.True(result);
        Assert.Equal(expectedSex, sex);
    }

    [Theory]
    [InlineData("123")] // Clearly invalid
    [InlineData("1234567890")] // Invalid control number
    [InlineData("abcdefghijk")] // Non-numeric
    [InlineData("")]            // Empty
    public void TryExtractBiologicalSex_InvalidNumbers_ReturnsFalse(string invalidNationalRegisterNumber)
    {
        // Act
        bool result = invalidNationalRegisterNumber.TryExtractBiologicalSex(out var sex);

        // Assert
        Assert.False(result);
        Assert.Null(sex);
    }
}

