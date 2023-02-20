using ItPlanet.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Infrastructure.Repositories.AnimalType;

public class AnimalTypeRepository : IAnimalTypeRepository
{
    private readonly ApiDbContext _dbContext;

    public AnimalTypeRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Domain.Models.AnimalType?> GetAsync(long id)
    {
        return _dbContext.AnimalTypes.FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<List<Domain.Models.AnimalType>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Domain.Models.AnimalType> CreateAsync(Domain.Models.AnimalType model)
    {
        var result = await _dbContext.AnimalTypes.AddAsync(model);
        await _dbContext.SaveChangesAsync();
        return result.Entity;
    }

    public Task CreateRangeAsync(IEnumerable<Domain.Models.AnimalType> models)
    {
        throw new NotImplementedException();
    }

    public async Task<Domain.Models.AnimalType> UpdateAsync(Domain.Models.AnimalType model)
    {
        var typeModel = await GetAsync(model.Id);
        typeModel!.Type = model.Type;

        await _dbContext.SaveChangesAsync();

        return typeModel;
    }

    public Task UpdateRangeAsync(IEnumerable<Domain.Models.AnimalType> models)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Domain.Models.AnimalType model)
    {
        throw new NotImplementedException();
    }

    public Task DeleteRangeAsync(IEnumerable<Domain.Models.AnimalType> models)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistAsync(long id)
    {
        return _dbContext.AnimalTypes.AnyAsync(x => x.Id == id);
    }

    public Task<Domain.Models.AnimalType?> GetByType(string type)
    {
        return _dbContext.AnimalTypes.FirstOrDefaultAsync(x => x.Type == type);
    }
}