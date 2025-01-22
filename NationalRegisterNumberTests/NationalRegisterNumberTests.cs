using FluentAssertions;
using NUnit.Framework;

namespace NationalRegisterNumber.UnitTests;

[TestFixture]
public class NationalRegisterNumberTests
{
    [TestCase("90022742191")]
    public void ValidateShouldReturnTrue(string nationalRegisterNumber)
    {
        // Act
        var result = NationalRegisterNumberGenerator.IsValid(nationalRegisterNumber);

        // Assert
        result.Should().BeTrue();
    }

    [TestCase("12345678910")]
    [TestCase("12345621748")]
    [TestCase("12345621777")]
    [TestCase("Test")]
    [TestCase("00000000000")]
    [TestCase("99999999999")]
    [TestCase("!@#!@%^@^@$^&@$^@sdfasdf")]
    [TestCase("$^@#^@##$44")]
    [TestCase("15435#$%4354dfsg")]
    [TestCase("90022742192")]
    public void ValidateShouldReturnFalse(string nationalRegisterNumber)
    {
        // Act
        var result = NationalRegisterNumberGenerator.IsValid(nationalRegisterNumber);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void GenerateWithBirthdateAndFollowNumberShouldWork()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        var followNumber = 16;

        // Act
        var result = NationalRegisterNumberGenerator.Generate(birthDate, followNumber);

        // Assert
        var assertion = NationalRegisterNumberGenerator.IsValid(result);
        assertion.Should().BeTrue();
    }

    [Test]
    public void GenerateShouldWork()
    {
        // Act
        var result = NationalRegisterNumberGenerator.Generate();

        // Assert
        var assertion = NationalRegisterNumberGenerator.IsValid(result);
        assertion.Should().BeTrue();
    }

    [Test]
    public void GenerateWithOldDateShouldThrowException()
    {
        // Arrange
        var reallyOldDate = new DateTime(1889, 12, 5);

        // Act
        var result = () => NationalRegisterNumberGenerator.Generate(reallyOldDate);

        // Assert
        result.Should().Throw<ArgumentException>();
    }

    [Test]
    public void GenerateWithFutureDateShouldThrowException()
    {
        // Arrange
        var futureDate = DateTime.Today.AddDays(10);

        // Act
        var result = () => NationalRegisterNumberGenerator.Generate(futureDate);

        // Assert
        result.Should().Throw<ArgumentException>();
    }

    [TestCase(0)]
    [TestCase(999)]
    [TestCase(10234)]
    [TestCase(-50)]
    public void GenerateWithWrongFollowNumberShouldThrowException(int followNumber)
    {
        // Arrange
        var validDate = new DateTime(1998, 1, 1);

        // Act
        var result = () => NationalRegisterNumberGenerator.Generate(validDate, followNumber);

        // Assert
        result.Should().Throw<ArgumentException>();
    }

    [Test]
    public void GenerateWithBirthDateShouldWork()
    {
        // Arrange
        var birthdate = new DateTime(2000, 1, 1);

        // Act
        var result = NationalRegisterNumberGenerator.Generate(birthdate);

        // Assert
        var assertion = NationalRegisterNumberGenerator.IsValid(result);
        assertion.Should().BeTrue();
    }

    [Test]
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

    [Test]
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

    [Test]
    public void GenerateWithBirthDateAndBiologicalSexShouldWork()
    {
        // Arrange
        var birthdate = new DateTime(2000, 1, 1);
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

    [Test]
    public void GenerateWithDateRangeShouldWork()
    {
        // Arrange
        var minDate = new DateTime(2000, 1, 1);
        var maxDate = new DateTime(2010, 12, 31);

        // Act
        var result = NationalRegisterNumberGenerator.Generate(minDate, maxDate);

        // Assert
        var assertion = NationalRegisterNumberGenerator.IsValid(result);
        assertion.Should().BeTrue();
    }

    [Test]
    public void GenerateWithWrongDateRangeShouldThrowError()
    {
        // Arrange
        var minDate = new DateTime(2005, 1, 1);
        var maxDate = new DateTime(1997, 12, 31);

        // Act
        var result = () => NationalRegisterNumberGenerator.Generate(minDate, maxDate);

        // Assert
        result.Should().Throw<ArgumentException>();
    }

    [Test]
    public void GenerateWithDateRangeAndBiologicalSexShouldWork()
    {
        // Arrange
        var minDate = new DateTime(2000, 1, 1);
        var maxDate = new DateTime(2010, 12, 31);
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

    [Test]
    public void GenerateWithBirthdateShouldWork()
    {
        // Arrange
        var birthDate = new DateTime(2020, 9, 16);

        // Act
        var result = NationalRegisterNumberGenerator.Generate(birthDate);

        // Assert
        var assertion = NationalRegisterNumberGenerator.IsValid(result);
        assertion.Should().BeTrue();
    }
}
