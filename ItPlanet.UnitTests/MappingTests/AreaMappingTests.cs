using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Models;

namespace ItPlanet.UnitTests.MappingTests;

public class AreaMappingTests
{
    [Fact]
    public void Map_FromCreateAreaDto_Success()
    {
        var mapper = MappingHelper.CreateMapper();

        var createAreaDto = new CreateAreaDto()
        {
            Name = "Name",
            AreaPoints = new List<LocationPointDto>() { new LocationPointDto() { Latitude = 10, Longitude = 10 } }
        };

        var area = mapper.Map<Area>(createAreaDto);

        area.Name.Should().Be("Name");
        area.AreaPoints.Should().Contain(x => x.Latitude == 10 && x.Longitude == 10);
    }
}