using ItPlanet.Database.DbContexts;
using ItPlanet.Models;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Database.Repositories.Animal;

public class AnimalRepository : IAnimalRepository
{
    private readonly ApiDbContext _dbContext;

    public AnimalRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<AnimalModel?> GetById(long id) => _dbContext.Animals.FirstOrDefaultAsync(x => x.Id == id);
}