using FluentAssertions;
using ItPlanet.Domain.Dto;

namespace ItPlanet.UnitTests;

public class CreateAreaDtoTest
{
    [Fact]
    public void IsValidArea_AreaContainsNullPoint_ReturnsFalse()
    {
        var area = new CreateAreaDto()
        {
            AreaPoints = new List<LocationPointDto>() { null!, null!, null! }
        };

        var isValid = area.IsValidArea();

        isValid.Should().BeFalse();
    }

    [Fact]
    public void IsValidArea_AreaContainsPointsDuplicates_ReturnsFalse()
    {
        var area = new CreateAreaDto()
        {
            AreaPoints = new List<LocationPointDto>()
            {
                new() { Latitude = 10, Longitude = 10 },
                new() { Latitude = 10, Longitude = 10 },
                new() { Latitude = 15, Longitude = 30 }
            }
        };

        var isValid = area.IsValidArea();

        isValid.Should().BeFalse();
    }
}