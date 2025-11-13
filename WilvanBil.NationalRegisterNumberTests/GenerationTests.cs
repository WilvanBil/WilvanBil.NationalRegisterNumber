using Xunit;

namespace WilvanBil.NationalRegisterNumber.UnitTests;

public class GenerationTests
{
    // Test constant for clarity
    private const int FollowNumberLastDigitIndex = 8;

    [Fact]
    public void GenerateWithBirthdateAndFollowNumberShouldWork()
    {
        // Arrange
        var birthDate = new DateOnly(1990, 1, 1);
        var followNumber = 16;

        // Act
        var result = NationalRegisterNumberGenerator.Generate(birthDate, followNumber);

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(result));
    }

    [Fact]
    public void GenerateShouldWork()
    {
        // Act
        var result = NationalRegisterNumberGenerator.Generate();

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(result));
    }

    [Fact]
    public void GenerateWithOldDateShouldThrowException()
    {
        // Arrange
        var reallyOldDate = new DateOnly(1889, 12, 5);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => NationalRegisterNumberGenerator.Generate(reallyOldDate));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    [InlineData(10234)]
    [InlineData(-50)]
    public void GenerateWithWrongFollowNumberShouldThrowException(int followNumber)
    {
        // Arrange
        var validDate = new DateOnly(1998, 1, 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => NationalRegisterNumberGenerator.Generate(validDate, followNumber));
    }

    [Fact]
    public void GenerateWithBirthDateShouldWork()
    {
        // Arrange
        var birthdate = new DateOnly(2000, 1, 1);

        // Act
        var result = NationalRegisterNumberGenerator.Generate(birthdate);

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(result));
    }

    [Fact]
    public void GenerateWithBiologicalSexFemaleShouldWork()
    {
        // Act
        var result = NationalRegisterNumberGenerator.Generate(BiologicalSex.Female);

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(result));
        var digit = int.Parse(result[FollowNumberLastDigitIndex].ToString());
        Assert.True(digit % 2 == 0, "female should have even last digit");
    }

    [Fact]
    public void GenerateWithBiologicalSexmaleShouldWork()
    {
        // Act
        var result = NationalRegisterNumberGenerator.Generate(BiologicalSex.Male);

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(result));
        var digit = int.Parse(result[FollowNumberLastDigitIndex].ToString());
        Assert.True(digit % 2 != 0, "male should have odd last digit");
    }

    [Fact]
    public void GenerateWithBirthDateAndBiologicalSexShouldWork()
    {
        // Arrange
        var birthdate = new DateOnly(2000, 1, 1);
        var sex = BiologicalSex.Male;

        // Act
        var result = NationalRegisterNumberGenerator.Generate(birthdate, sex);

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(result));
        var digit = int.Parse(result[FollowNumberLastDigitIndex].ToString());
        Assert.True(digit % 2 != 0, "male should have odd last digit");
    }

    [Fact]
    public void GenerateWithDateRangeShouldWork()
    {
        // Arrange
        var minDate = new DateOnly(2000, 1, 1);
        var maxDate = new DateOnly(2010, 12, 31);

        // Act
        var result = NationalRegisterNumberGenerator.Generate(minDate, maxDate);

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(result));
    }

    [Fact]
    public void GenerateWithWrongDateRangeShouldThrowError()
    {
        // Arrange
        var minDate = new DateOnly(2005, 1, 1);
        var maxDate = new DateOnly(1997, 12, 31);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => NationalRegisterNumberGenerator.Generate(minDate, maxDate));
    }

    [Fact]
    public void GenerateWithDateRangeAndBiologicalSexShouldWork()
    {
        // Arrange
        var minDate = new DateOnly(2000, 1, 1);
        var maxDate = new DateOnly(2010, 12, 31);
        var sex = BiologicalSex.Female;

        // Act
        var result = NationalRegisterNumberGenerator.Generate(minDate, maxDate, sex);

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(result));
        var digit = int.Parse(result[FollowNumberLastDigitIndex].ToString());
        Assert.True(digit % 2 == 0, "female should have even last digit");
    }

    [Fact]
    public void GenerateWithBirthdateShouldWork()
    {
        // Arrange
        var birthDate = new DateOnly(2020, 9, 16);

        // Act
        var result = NationalRegisterNumberGenerator.Generate(birthDate);

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(result));
    }

    [Theory]
    [InlineData(1, BiologicalSex.Male)]    // Minimum valid, odd = male
    [InlineData(2, BiologicalSex.Female)]  // Minimum even = female
    [InlineData(997, BiologicalSex.Male)]  // Maximum odd = male
    [InlineData(998, BiologicalSex.Female)] // Maximum valid, even = female
    public void GenerateWithBoundaryFollowNumbers_ShouldWorkAndMatchSex(int followNumber, BiologicalSex expectedSex)
    {
        // Arrange
        var birthDate = new DateOnly(2000, 1, 1);

        // Act
        var result = NationalRegisterNumberGenerator.Generate(birthDate, followNumber);

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(result));
        
        // Verify sex encoding (digit at position 8)
        var digit = int.Parse(result[FollowNumberLastDigitIndex].ToString());
        var isEven = digit % 2 == 0;
        
        if (expectedSex == BiologicalSex.Female)
            Assert.True(isEven, "female should have even last digit");
        else
            Assert.False(isEven, "male should have odd last digit");
    }

    [Theory]
    [InlineData(1)]   // Minimum boundary
    [InlineData(500)] // Middle value
    [InlineData(998)] // Maximum boundary
    public void GenerateWithValidFollowNumbers_ShouldProduceValidNumbers(int followNumber)
    {
        // Arrange
        var birthDate = new DateOnly(1995, 6, 15);

        // Act
        var result = NationalRegisterNumberGenerator.Generate(birthDate, followNumber);

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(result));
    }

    [Theory]
    [InlineData(2000, 2, 29)] // Valid leap year (divisible by 400)
    [InlineData(2024, 2, 29)] // Valid leap year (divisible by 4, not 100)
    [InlineData(2020, 2, 29)] // Valid leap year
    [InlineData(1996, 2, 29)] // Valid leap year
    public void GenerateWithLeapYearDate_ShouldWork(int year, int month, int day)
    {
        // Arrange
        var birthDate = new DateOnly(year, month, day);

        // Act
        var result = NationalRegisterNumberGenerator.Generate(birthDate);

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(result));
        
        // Verify we can extract the date back
        Assert.True(result.TryExtractBirthDate(out var extractedDate));
        Assert.Equal(birthDate, extractedDate);
    }

    [Fact]
    public void GenerateWith1900LeapYear_ShouldThrowOrFail() =>
        // Note: 1900-02-29 is not a valid date because 1900 is not a leap year
        // (divisible by 100 but not by 400)
        // DateOnly constructor itself will throw ArgumentOutOfRangeException

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new DateOnly(1900, 2, 29));

    [Theory]
    [InlineData(2001, 2, 28)] // Non-leap year, last valid day
    [InlineData(1999, 2, 28)] // Non-leap year
    [InlineData(1900, 2, 28)] // Century non-leap year
    public void GenerateWithNonLeapYearFebruary_ShouldWork(int year, int month, int day)
    {
        // Arrange
        var birthDate = new DateOnly(year, month, day);

        // Act
        var result = NationalRegisterNumberGenerator.Generate(birthDate);

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(result));
    }
}
