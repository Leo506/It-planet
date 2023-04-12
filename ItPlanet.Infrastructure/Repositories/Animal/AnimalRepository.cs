using System.Text.Json;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Geometry;
using ItPlanet.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ItPlanet.Infrastructure.Repositories.Animal;

public class AnimalRepository : IAnimalRepository
{
    private readonly ApiDbContext _dbContext;
    private readonly ILogger<AnimalRepository> _logger;

    public AnimalRepository(ApiDbContext dbContext, ILogger<AnimalRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
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

    public async Task<IEnumerable<Domain.Models.Animal>> GetAnimalsChippedInArea(IEnumerable<Segment> area, DateTime startDate, DateTime endDate)
    {
        var animals = await _dbContext.Animals
            .Include(x => x.ChippingLocation)
            .Include(x => x.VisitedPoints)
            .Include(x => x.Types)
            .Where(x => x.ChippingDateTime >= startDate && x.ChippingDateTime <= endDate)
            .ToListAsync().ConfigureAwait(false);

        return animals.Where(x => x.ChippingLocation.ToPoint().IsInside(area) && x.VisitedPoints.Count == 0)
            .DistinctBy(x => x.Id);
    }

    public async Task<IEnumerable<Domain.Models.Animal>> GetAnimalsThatVisitArea(IEnumerable<Segment> area,
        DateTime startDate, DateTime endDate)
    {
        var visitedPoints = await GetVisitedPointsInInterval(startDate, endDate).ConfigureAwait(false);
        _logger.LogInformation("Visited points:");
        foreach (var p in visitedPoints)
        {
            _logger.LogInformation(JsonSerializer.Serialize(p.LocationPoint));
        }
        
        return visitedPoints
            .Where(x => x.LocationPoint.ToPoint().IsInside(area))
            .Select(x => x.Animal)
            .DistinctBy(x => x.Id);
    }

    public async Task<IEnumerable<Domain.Models.Animal>> GetAnimalsThatVisitAreaIncludingEdge(IEnumerable<Segment> area, DateTime startDate, DateTime endDate)
    {
        var visitedPoints = await GetVisitedPointsInInterval(startDate, endDate).ConfigureAwait(false);
        return visitedPoints
            .Where(x => x.LocationPoint.ToPoint().IsInsideOrOnEdge(area))
            .Select(x => x.Animal);
    }

    public async Task<IEnumerable<Domain.Models.Animal>> GetAnimalsThatDoNotVisitArea(IEnumerable<Segment> area, DateTime startDate, DateTime endDate)
    {
        var visitedPoints = await GetVisitedPointsInInterval(startDate, endDate).ConfigureAwait(false);
        return visitedPoints
            .Where(x => x.LocationPoint.ToPoint().IsInsideOrOnEdge(area) is false)
            .Select(x => x.Animal);
    }

    private async Task<IEnumerable<Domain.Models.VisitedPoint>> GetVisitedPointsInInterval(DateTime startDate,
        DateTime endDate)
    {
        var visitedPoints = await _dbContext.VisitedPoints
            .Include(x => x.Animal)
                .ThenInclude(x => x.Types)
            .Include(x => x.LocationPoint)
            .Where(x => x.DateTimeOfVisitLocationPoint >= startDate && x.DateTimeOfVisitLocationPoint <= endDate)
            .ToListAsync().ConfigureAwait(false);

        return visitedPoints;
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

    public Task CreateRangeAsync(params Domain.Models.Animal[] models)
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