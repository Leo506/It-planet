using System.Text.Json;
using ItPlanet.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ItPlanet.Infrastructure.Repositories.VisitedPoint;

public class VisitedPointRepository : IVisitedPointRepository
{
    private readonly ApiDbContext _dbContext;
    private readonly ILogger<VisitedPointRepository> _logger;

    public VisitedPointRepository(ApiDbContext dbContext, ILogger<VisitedPointRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<Domain.Models.VisitedPoint?> GetAsync(long id)
    {
        return _dbContext.VisitedPoints.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Domain.Models.VisitedPoint> CreateAsync(Domain.Models.VisitedPoint model)
    {
        var entity = await _dbContext.AddAsync(model);
        await _dbContext.SaveChangesAsync();
        return entity.Entity;
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

    public async Task DeleteAsync(Domain.Models.VisitedPoint model)
    {
        _dbContext.VisitedPoints.Remove(model);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Domain.Models.VisitedPoint>> GetVisitedPointsInInterval(DateTime startDate,
        DateTime endDate)
    {
        await foreach (var point in _dbContext.VisitedPoints)
        {
            _logger.LogInformation(JsonSerializer.Serialize(point));
        }
        
        var points = await _dbContext.VisitedPoints
            .Include(x => x.Animal)
            .Include(x => x.LocationPoint)
            .Where(x => x.DateTimeOfVisitLocationPoint >= startDate && x.DateTimeOfVisitLocationPoint <= endDate)
            .ToListAsync();

        return points;
    }
}