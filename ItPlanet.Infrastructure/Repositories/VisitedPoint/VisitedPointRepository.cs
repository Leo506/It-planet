using ItPlanet.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Infrastructure.Repositories.VisitedPoint;

public class VisitedPointRepository : IVisitedPointRepository
{
    private readonly ApiDbContext _dbContext;

    public VisitedPointRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Domain.Models.VisitedPoint> CreateAsync(Domain.Models.VisitedPoint model)
    {
        var entity = await _dbContext.AddAsync(model);
        await _dbContext.SaveChangesAsync();
        return entity.Entity;
    }

    public async Task<Domain.Models.VisitedPoint> UpdateAsync(Domain.Models.VisitedPoint model)
    {
        var point = await _dbContext.VisitedPoints.FirstOrDefaultAsync(x => x.Id == model.Id)
            .ConfigureAwait(false);

        point!.AnimalId = model.AnimalId;
        point.LocationPointId = model.LocationPointId;
        point.DateTimeOfVisitLocationPoint = model.DateTimeOfVisitLocationPoint;

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        return point;
    }

    public async Task DeleteAsync(Domain.Models.VisitedPoint model)
    {
        _dbContext.VisitedPoints.Remove(model);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}