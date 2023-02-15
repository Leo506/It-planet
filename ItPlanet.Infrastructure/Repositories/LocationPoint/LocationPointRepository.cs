﻿using ItPlanet.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Infrastructure.Repositories.LocationPoint;

public class LocationPointRepository : ILocationPointRepository
{
    private readonly ApiDbContext _dbContext;

    public LocationPointRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Domain.Models.LocationPoint?> GetAsync(long id) =>
        _dbContext.LocationPoints.FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<Domain.Models.LocationPoint>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Models.LocationPoint> CreateAsync(Domain.Models.LocationPoint model)
    {
        throw new NotImplementedException();
    }

    public Task CreateRangeAsync(IEnumerable<Domain.Models.LocationPoint> models)
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Models.LocationPoint> UpdateAsync(Domain.Models.LocationPoint model)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRangeAsync(IEnumerable<Domain.Models.LocationPoint> models)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Domain.Models.LocationPoint model)
    {
        throw new NotImplementedException();
    }

    public Task DeleteRangeAsync(IEnumerable<Domain.Models.LocationPoint> models)
    {
        throw new NotImplementedException();
    }
}