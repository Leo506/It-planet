using ItPlanet.Database.DbContexts;
using ItPlanet.Dto;
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
        return _dbContext.Animals
            .Include(x => x.VisitedPoints)
            .Include(x => x.Types)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Models.Animal>> SearchAsync(SearchAnimalDto search)
    {
        search.StartDateTime ??= DateTime.MinValue;
        search.EndDateTime ??=DateTime.MaxValue;

        var animals = _dbContext.Animals.Where(x =>
            x.ChippingDateTime >= search.StartDateTime &&
            x.ChippingDateTime <= search.EndDateTime);

        if (search.ChipperId is not null)
            animals = animals.Where(x => x.ChipperId == search.ChipperId);

        if (search.ChippingLocationId is not null)
            animals = animals.Where(x => x.ChippingLocationId == search.ChippingLocationId);

        if (search.LifeStatus is not null)
            animals = animals.Where(x => x.LifeStatus == search.LifeStatus);

        if (search.Gender is not null)
            animals = animals.Where(x => x.Gender == search.Gender);

        animals = animals.Include(x => x.Types).Include(x => x.VisitedPoints);

        return await animals.Skip(search.From).Take(search.Size).ToListAsync();
    }
}