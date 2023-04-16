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
        return _dbContext.AnimalTypes
            .Include(x => x.Animals)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Domain.Models.AnimalType> CreateAsync(Domain.Models.AnimalType model)
    {
        var result = await _dbContext.AnimalTypes.AddAsync(model);
        await _dbContext.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Domain.Models.AnimalType> UpdateAsync(Domain.Models.AnimalType model)
    {
        var typeModel = await GetAsync(model.Id);
        typeModel!.Type = model.Type;

        await _dbContext.SaveChangesAsync();

        return typeModel;
    }

    public async Task DeleteAsync(Domain.Models.AnimalType model)
    {
        _dbContext.AnimalTypes.Remove(model);
        await _dbContext.SaveChangesAsync();
    }

    public Task<bool> ExistAsync(long id) => _dbContext.AnimalTypes.AnyAsync(x => x.Id == id);

    public Task<bool> ExistAsync(string typeName) => _dbContext.AnimalTypes.AnyAsync(x => x.Type == typeName);
}