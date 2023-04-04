using ItPlanet.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Infrastructure.Repositories.VisitedPoint;

public class VisitedPointRepository : IVisitedPointsRepository
{
    private readonly ApiDbContext _dbContext;

    public VisitedPointRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Domain.Models.VisitedPoint?> GetAsync(long id)
    {
        return _dbContext.VisitedPoints.FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<List<Domain.Models.VisitedPoint>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Domain.Models.VisitedPoint> CreateAsync(Domain.Models.VisitedPoint model)
    {
        var entity = await _dbContext.AddAsync(model);
        await _dbContext.SaveChangesAsync();
        return entity.Entity;
    }

    public Task CreateRangeAsync(params Domain.Models.VisitedPoint[] models)
    {
        throw new NotImplementedException();
    }

    public async Task<Domain.Models.VisitedPoint> UpdateAsync(Domain.Models.VisitedPoint model)
    {
        var point = await GetAsync(model.Id);

        point!.AnimalId = model.AnimalId;
        point.LocationPointId = model.LocationPointId;
        point.DateTimeOfVisitLocationPoint = model.DateTimeOfVisitLocationPoint;

        await _dbContext.SaveChangesAsync();

        return point;
    }

    public Task UpdateRangeAsync(IEnumerable<Domain.Models.VisitedPoint> models)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Domain.Models.VisitedPoint model)
    {
        _dbContext.VisitedPoints.Remove(model);
        await _dbContext.SaveChangesAsync();
    }

    public Task DeleteRangeAsync(IEnumerable<Domain.Models.VisitedPoint> models)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistAsync(long id)
    {
        throw new NotImplementedException();
    }
}