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

    public Task<Domain.Models.AnimalType?> GetAsync(long id) =>
        _dbContext.AnimalTypes.FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<Domain.Models.AnimalType>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Models.AnimalType> CreateAsync(Domain.Models.AnimalType model)
    {
        throw new NotImplementedException();
    }

    public Task CreateRangeAsync(IEnumerable<Domain.Models.AnimalType> models)
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Models.AnimalType> UpdateAsync(Domain.Models.AnimalType model)
    {
        throw new NotImplementedException();
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
}