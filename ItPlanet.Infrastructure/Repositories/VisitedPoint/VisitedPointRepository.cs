using ItPlanet.Infrastructure.DatabaseContext;

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
        throw new NotImplementedException();
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

    public Task CreateRangeAsync(IEnumerable<Domain.Models.VisitedPoint> models)
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Models.VisitedPoint> UpdateAsync(Domain.Models.VisitedPoint model)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRangeAsync(IEnumerable<Domain.Models.VisitedPoint> models)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Domain.Models.VisitedPoint model)
    {
        throw new NotImplementedException();
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