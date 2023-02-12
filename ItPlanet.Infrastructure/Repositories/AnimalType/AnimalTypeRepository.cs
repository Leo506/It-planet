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

    public Task<Domain.Models.AnimalType?> GetTypeAsync(long id)
    {
        return _dbContext.AnimalTypes.FirstOrDefaultAsync(x => x.Id == id);
    }
}