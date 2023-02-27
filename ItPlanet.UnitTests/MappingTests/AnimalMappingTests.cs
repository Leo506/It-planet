using AutoFixture;
using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Models;

namespace ItPlanet.UnitTests.MappingTests;

public class AnimalMappingTests
{
    [Fact]
    public void Map_FromUpdateAnimalDto_Success()
    {
        var mapper = MappingHelper.CreateMapper();
        
        var updateDto = new Fixture().Create<UpdateAnimalDto>();

        var animal = mapper.Map<Animal>(updateDto);

        animal.Should().NotBeNull();
    }

    [Fact]
    public void Map_FromAnimalDto_Success()
    {
        var mapper = MappingHelper.CreateMapper();

        var dto = new Fixture().Create<AnimalDto>();

        var animal = mapper.Map<Animal>(dto);

        animal.Should().NotBeNull();
    }
}