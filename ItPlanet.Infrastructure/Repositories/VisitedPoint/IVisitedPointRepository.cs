namespace ItPlanet.Infrastructure.Repositories.VisitedPoint;

public interface IVisitedPointRepository
{
    Task<Domain.Models.VisitedPoint> CreateAsync(Domain.Models.VisitedPoint model);
    Task<Domain.Models.VisitedPoint> UpdateAsync(Domain.Models.VisitedPoint model);
    Task DeleteAsync(Domain.Models.VisitedPoint model);
}