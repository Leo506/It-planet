using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Models;

namespace ItPlanet.UnitTests.ValidationTests;

public class AnimalDtoValidationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IsValid_InvalidAnimalTypeId_ReturnsFalse(long animalTypeId)
    {
        var sut = new AnimalDto
        {
            Gender = GenderConstants.Male,
            AnimalTypes = new[] { animalTypeId }
        };

        var isValid = sut.IsValid();

        isValid.Should().BeFalse();
    }

    [Fact]
    public void IsValid_InvalidGenderProperty_ReturnsFalse()
    {
        var sut = new AnimalDto
        {
            Gender = "Invalid gender"
        };

        var isValid = sut.IsValid();

        isValid.Should().BeFalse();
    }
}