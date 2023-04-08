namespace ItPlanet.Web.Services.Area;

public interface IAreaService
{
    Task<Domain.Models.Area> CreateAreaAsync(Domain.Models.Area area);
}