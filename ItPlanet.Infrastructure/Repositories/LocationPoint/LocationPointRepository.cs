using ItPlanet.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Infrastructure.Repositories.LocationPoint;

public class LocationPointRepository : ILocationPointRepository
{
    private readonly ApiDbContext _dbContext;

    public LocationPointRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Domain.Models.LocationPoint?> GetAsync(long id)
    {
        return _dbContext.LocationPoints.FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<List<Domain.Models.LocationPoint>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Domain.Models.LocationPoint> CreateAsync(Domain.Models.LocationPoint model)
    {
        var result = await _dbContext.LocationPoints.AddAsync(model).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync();
        return result.Entity;
    }

    public Task CreateRangeAsync(IEnumerable<Domain.Models.LocationPoint> models)
    {
        throw new NotImplementedException();
    }

    public async Task<Domain.Models.LocationPoint> UpdateAsync(Domain.Models.LocationPoint model)
    {
        var point = await GetAsync(model.Id);
        point!.Latitude = model.Latitude;
        point.Longitude = model.Longitude;

        await _dbContext.SaveChangesAsync();

        return point;
    }

    public Task UpdateRangeAsync(IEnumerable<Domain.Models.LocationPoint> models)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Domain.Models.LocationPoint model)
    {
        _dbContext.LocationPoints.Remove(model);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public Task DeleteRangeAsync(IEnumerable<Domain.Models.LocationPoint> models)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistAsync(long id)
    {
        return _dbContext.LocationPoints.AnyAsync(x => x.Id == id);
    }

    public Task<Domain.Models.LocationPoint?> GetPointByCoordinateAsync(double pointLatitude, double pointLongitude)
    {
        const double precision = 0.00001;
        return _dbContext.LocationPoints.FirstOrDefaultAsync(x =>
            Math.Abs(x.Latitude - pointLatitude) <= precision && Math.Abs(x.Longitude - pointLongitude) <= precision);
    }
}