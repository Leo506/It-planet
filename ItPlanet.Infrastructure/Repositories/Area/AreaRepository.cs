using ItPlanet.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Infrastructure.Repositories.Area;

public class AreaRepository : IAreaRepository
{
    private readonly ApiDbContext _dbContext;

    public AreaRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
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
        await _dbContext.SaveChangesAsync();

        return areaEntity.Entity;
    }
}