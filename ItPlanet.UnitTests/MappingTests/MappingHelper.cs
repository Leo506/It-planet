using AutoMapper;
using ItPlanet.Web.Mapping;

namespace ItPlanet.UnitTests.MappingTests;

public static class MappingHelper
{
    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(c => c.AddProfile(typeof(AutoMapperProfile)));
        return config.CreateMapper();
    }
}