using ItPlanet.Database.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Database.Repositories.Animal;

public class AnimalRepository : IAnimalRepository
{
    private readonly ApiDbContext _dbContext;

    public AnimalRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Models.Animal?> GetByIdAsync(long id)
    {
        return _dbContext.Animals.FirstOrDefaultAsync(x => x.Id == id);
    }
}