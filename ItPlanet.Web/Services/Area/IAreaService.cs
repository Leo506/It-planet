namespace ItPlanet.Web.Services.Area;

public interface IAreaService
{
    Task<ItPlanet.Domain.Models.Area> GetAreaById(long id);
    Task<Domain.Models.Area> CreateAreaAsync(Domain.Models.Area area);
    Task DeleteAreaById(long areaId);
    Task<Domain.Models.Area> UpdateArea(long areaId, Domain.Models.Area area);
}