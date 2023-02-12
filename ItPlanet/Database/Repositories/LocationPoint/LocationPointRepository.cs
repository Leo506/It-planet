using ItPlanet.Database.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Database.Repositories.LocationPoint;

public class LocationPointRepository : ILocationPointRepository
{
    private readonly ApiDbContext _dbContext;

    public LocationPointRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Models.LocationPoint?> GetByIdAsync(long id)
    {
        return _dbContext.LocationPoints.FirstOrDefaultAsync(x => x.Id == id);
    }
}