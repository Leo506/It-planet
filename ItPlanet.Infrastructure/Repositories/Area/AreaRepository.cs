using ItPlanet.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ItPlanet.Infrastructure.Repositories.Area;

public class AreaRepository : IAreaRepository
{
    private readonly ApiDbContext _dbContext;
    private readonly ILogger<AreaRepository> _logger;

    public AreaRepository(ApiDbContext dbContext, ILogger<AreaRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<Domain.Models.Area>> GetAllAsync()
    {
        var areas = await _dbContext.Areas
            .Include(x => x.AreaPoints)
            .ToListAsync();
        return areas;
    }

    public async Task<Domain.Models.Area> CreateAsync(Domain.Models.Area areaModel)
    {
        var areaEntity = await _dbContext.Areas.AddAsync(areaModel);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        _logger.LogInformation($"Area with id {areaEntity.Entity.Id} was added [{DateTime.Now}]");
        return areaEntity.Entity;
    }

    public Task<bool> ExistAsync(string areaName)
    {
        return _dbContext.Areas.AnyAsync(x => x.Name == areaName);
    }

    public Task<Domain.Models.Area?> GetAsync(long id)
    {
        return _dbContext.Areas
            .Include(x => x.AreaPoints)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task DeleteAsync(Domain.Models.Area area)
    {
        _dbContext.Areas.Remove(area);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        _logger.LogInformation($"Area with id {area.Id} was deleted [{DateTime.Now}]");
    }

    public async Task<Domain.Models.Area> UpdateAsync(Domain.Models.Area area)
    {
        await _dbContext.AreaPoints.Where(x => x.AreaId == area.Id).ExecuteDeleteAsync().ConfigureAwait(false);
        foreach (var point in area.AreaPoints)
        {
            point.AreaId = area.Id;
            await _dbContext.AreaPoints.AddAsync(point).ConfigureAwait(false);
        }
        
        var updatedEntity = await GetAsync(area.Id).ConfigureAwait(false)!;
        updatedEntity.Name = area.Name;
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return area;
    }
}