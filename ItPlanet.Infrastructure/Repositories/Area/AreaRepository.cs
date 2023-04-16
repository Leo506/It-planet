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
    }

    public async Task<Domain.Models.Area> UpdateAsync(Domain.Models.Area area)
    {
        DeleteOldAreaPoints(area);
        await CreateNewAreaPoints(area).ConfigureAwait(false);
        
        await RenameArea(area).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return area;
    }

    private void DeleteOldAreaPoints(Domain.Models.Area area)
    {
        foreach (var areaPoint in _dbContext.AreaPoints.Where(x => x.AreaId == area.Id))
            _dbContext.AreaPoints.Remove(areaPoint);
    }

    private async Task CreateNewAreaPoints(Domain.Models.Area area)
    {
        foreach (var point in area.AreaPoints)
        {
            point.AreaId = area.Id;
            await _dbContext.AreaPoints.AddAsync(point).ConfigureAwait(false);
        }
    }
    
    private async Task RenameArea(Domain.Models.Area area)
    {
        var updatedEntity = await GetAsync(area.Id).ConfigureAwait(false)!;
        updatedEntity.Name = area.Name;
    }
}