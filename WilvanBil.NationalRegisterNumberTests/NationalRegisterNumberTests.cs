using FluentAssertions;
using FsCheck.Xunit;
using Xunit;

namespace WilvanBil.NationalRegisterNumber.UnitTests;

public class NationalRegisterNumberTests
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

    /// <summary>
    /// Ensures the NationalRegisterNumberGenerator.Generate method produces valid results for any DateTime input.
    /// FsCheck generates a wide range of random and edge-case DateTime values for testing.
    /// </summary>
    /// <param name="datetime">The randomly generated DateTime input.</param>
    [Property(Verbose = true)]
    public void GenerateShouldWorkForAnyDateTime(DateTime datetime)
    {
        // Act
        var nationalRegisterNumber = NationalRegisterNumberGenerator.Generate(DateOnly.FromDateTime(datetime));

        // Assert
        Assert.True(NationalRegisterNumberGenerator.IsValid(nationalRegisterNumber));
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
