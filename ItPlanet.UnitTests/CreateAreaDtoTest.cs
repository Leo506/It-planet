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
}