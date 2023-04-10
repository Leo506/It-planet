namespace ItPlanet.Infrastructure.Repositories.VisitedPoint;

public interface IVisitedPointsRepository : IRepository<Domain.Models.VisitedPoint, long>
{
    Task<IEnumerable<Domain.Models.VisitedPoint>> GetVisitedPointsInInterval(DateTime startDate, DateTime endDate);
}