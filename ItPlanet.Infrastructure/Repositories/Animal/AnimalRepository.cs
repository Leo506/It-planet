using ItPlanet.Dto;
using ItPlanet.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Infrastructure.Repositories.Animal;

public class AnimalRepository : IAnimalRepository
{
    private readonly ApiDbContext _dbContext;

    public AnimalRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Domain.Models.Animal>> SearchAsync(SearchAnimalDto search)
    {
        search.StartDateTime ??= DateTime.MinValue;
        search.EndDateTime ??= DateTime.MaxValue;

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

        animals = animals
            .Include(x => x.Types)
            .Include(x => x.VisitedPoints)
            .Include(x => x.Chipper);

        return await animals.Skip(search.From).Take(search.Size).ToListAsync().ConfigureAwait(false);
    }

    public async Task<Domain.Models.Animal> AddTypeAsync(long animalId, Domain.Models.AnimalType type)
    {
        var animal = await GetAsync(animalId);
        animal!.Types.Add(type);

        await _dbContext.SaveChangesAsync();

        return animal;
    }

    public async Task<Domain.Models.Animal> ReplaceTypeAsync(long animalId, Domain.Models.AnimalType oldType,
        Domain.Models.AnimalType newType)
    {
        var animal = await GetAsync(animalId);

        animal!.Types.Remove(oldType);
        animal.Types.Add(newType);

        await _dbContext.SaveChangesAsync();

        return animal;
    }

    public async Task<Domain.Models.Animal> DeleteTypeAsync(long animalId, Domain.Models.AnimalType type)
    {
        var animal = await GetAsync(animalId);

        animal!.Types.Remove(type);

        await _dbContext.SaveChangesAsync();

        return animal;
    }

    public Task<Domain.Models.Animal?> GetAsync(long id)
    {
        return _dbContext.Animals
            .Include(x => x.VisitedPoints)
            .Include(x => x.Types)
            .Include(x => x.Chipper)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<List<Domain.Models.Animal>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Domain.Models.Animal> CreateAsync(Domain.Models.Animal model)
    {
        var animal = await _dbContext.Animals.AddAsync(model).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return animal.Entity;
    }

    public Task CreateRangeAsync(IEnumerable<Domain.Models.Animal> models)
    {
        throw new NotImplementedException();
    }

    public async Task<Domain.Models.Animal> UpdateAsync(Domain.Models.Animal model)
    {
        var animal = await GetAsync(model.Id);

        animal!.Weight = model.Weight;
        animal.Height = model.Height;
        animal.LifeStatus = model.LifeStatus;
        animal.Length = model.Length;
        animal.ChipperId = model.ChipperId;
        animal.Gender = model.Gender;
        animal.DeathDateTime = model.DeathDateTime;
        animal.ChippingLocationId = model.ChippingLocationId;

        await _dbContext.SaveChangesAsync();

        return animal;
    }

    public Task UpdateRangeAsync(IEnumerable<Domain.Models.Animal> models)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Domain.Models.Animal model)
    {
        _dbContext.Animals.Remove(model);
        await _dbContext.SaveChangesAsync();
    }

    public Task DeleteRangeAsync(IEnumerable<Domain.Models.Animal> models)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistAsync(long id)
    {
        return _dbContext.Animals.AnyAsync(x => x.Id == id);
    }
}