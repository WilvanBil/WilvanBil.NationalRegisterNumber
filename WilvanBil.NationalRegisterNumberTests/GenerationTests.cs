using FluentAssertions;
using Xunit;

namespace WilvanBil.NationalRegisterNumber.UnitTests;

public class GenerationTests
{
    [Fact]
    public void GenerateWithBirthdateAndFollowNumberShouldWork()
    {
        // Arrange
        var birthDate = new DateOnly(1990, 1, 1);
        var followNumber = 16;

        // Act
        var result = NationalRegisterNumberGenerator.Generate(birthDate, followNumber);

        // Assert
        var assertion = NationalRegisterNumberGenerator.IsValid(result);
        assertion.Should().BeTrue();
    }

    [Fact]
    public void GenerateShouldWork()
    {
        // Act
        var result = NationalRegisterNumberGenerator.Generate();

        // Assert
        var assertion = NationalRegisterNumberGenerator.IsValid(result);
        assertion.Should().BeTrue();
    }

    [Fact]
    public void GenerateWithOldDateShouldThrowException()
    {
        // Arrange
        var reallyOldDate = new DateOnly(1889, 12, 5);

        // Act
        var result = () => NationalRegisterNumberGenerator.Generate(reallyOldDate);

        // Assert
        result.Should().Throw<ArgumentException>();
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

        // Act
        var result = () => NationalRegisterNumberGenerator.Generate(validDate, followNumber);

        // Assert
        result.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GenerateWithBirthDateShouldWork()
    {
        // Arrange
        var birthdate = new DateOnly(2000, 1, 1);

        // Act
        var result = NationalRegisterNumberGenerator.Generate(birthdate);

        // Assert
        var assertion = NationalRegisterNumberGenerator.IsValid(result);
        assertion.Should().BeTrue();
    }

    [Fact]
    public void GenerateWithBiologicalSexFemaleShouldWork()
    {
        // Act
        var result = NationalRegisterNumberGenerator.Generate(BiologicalSex.Female);

        // Assert
        var assertion = NationalRegisterNumberGenerator.IsValid(result);
        assertion.Should().BeTrue();
        var digit = int.Parse(result[8].ToString());
        var even = digit % 2 == 0;
        even.Should().BeTrue();
    }

    [Fact]
    public void GenerateWithBiologicalSexmaleShouldWork()
    {
        // Act
        var result = NationalRegisterNumberGenerator.Generate(BiologicalSex.Male);

        // Assert
        var assertion = NationalRegisterNumberGenerator.IsValid(result);
        assertion.Should().BeTrue();
        var digit = int.Parse(result[8].ToString());
        var uneven = digit % 2 != 0;
        uneven.Should().BeTrue();
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
        var assertion = NationalRegisterNumberGenerator.IsValid(result);
        assertion.Should().BeTrue();
        var digit = int.Parse(result[8].ToString());
        var uneven = digit % 2 != 0;
        uneven.Should().BeTrue();
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
        var assertion = NationalRegisterNumberGenerator.IsValid(result);
        assertion.Should().BeTrue();
    }

    [Fact]
    public void GenerateWithWrongDateRangeShouldThrowError()
    {
        // Arrange
        var minDate = new DateOnly(2005, 1, 1);
        var maxDate = new DateOnly(1997, 12, 31);

        // Act
        var result = () => NationalRegisterNumberGenerator.Generate(minDate, maxDate);

        // Assert
        result.Should().Throw<ArgumentException>();
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
        var assertion = NationalRegisterNumberGenerator.IsValid(result);
        assertion.Should().BeTrue();
        var digit = int.Parse(result[8].ToString());
        var even = digit % 2 == 0;
        even.Should().BeTrue();
    }

    [Fact]
    public void GenerateWithBirthdateShouldWork()
    {
        // Arrange
        var birthDate = new DateOnly(2020, 9, 16);

        // Act
        var result = NationalRegisterNumberGenerator.Generate(birthDate);

        // Assert
        var assertion = NationalRegisterNumberGenerator.IsValid(result);
        assertion.Should().BeTrue();
    }
}
