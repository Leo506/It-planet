using ItPlanet.Database.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Database.Repositories.AnimalType;

public class AnimalTypeRepository : IAnimalTypeRepository
{
    private readonly ApiDbContext _dbContext;

    public AnimalTypeRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Models.AnimalType?> GetTypeAsync(long id) =>
        _dbContext.AnimalTypes.FirstOrDefaultAsync(x => x.Id == id);
}