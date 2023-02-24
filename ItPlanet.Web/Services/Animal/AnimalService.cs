using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Domain.Models;
using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.Account;
using ItPlanet.Infrastructure.Repositories.Animal;
using ItPlanet.Infrastructure.Repositories.AnimalType;
using ItPlanet.Infrastructure.Repositories.LocationPoint;

namespace ItPlanet.Web.Services.Animal;

public class AnimalService : IAnimalService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAnimalRepository _animalRepository;
    private readonly IAnimalTypeRepository _animalTypeRepository;
    private readonly ILocationPointRepository _locationPointRepository;

    public AnimalService(IAnimalRepository animalRepository, IAnimalTypeRepository animalTypeRepository,
        IAccountRepository accountRepository, ILocationPointRepository locationPointRepository)
    {
        _animalRepository = animalRepository;
        _animalTypeRepository = animalTypeRepository;
        _accountRepository = accountRepository;
        _locationPointRepository = locationPointRepository;
    }

    public async Task<Domain.Models.Animal> GetAnimalAsync(long id)
    {
        var animal = await _animalRepository.GetAsync(id);
        return animal ?? throw new AnimalNotFoundException(id);
    }

    public Task<IEnumerable<Domain.Models.Animal>> SearchAnimalAsync(SearchAnimalDto searchAnimalDto)
    {
        return _animalRepository.SearchAsync(searchAnimalDto);
    }

    public async Task<IEnumerable<VisitedPoint>> GetAnimalVisitedPoints(long animalId,
        VisitedLocationDto visitedLocationDto)
    {
        var animal = await GetAnimalAsync(animalId);
        visitedLocationDto.StarDateTime ??= DateTime.MinValue;
        visitedLocationDto.EndDateTime ??= DateTime.MaxValue;

        var visitedPoints = animal.VisitedPoints;
        var result = visitedPoints.Where(x => x.DateTimeOfVisitLocationPoint >= visitedLocationDto.StarDateTime &&
                                              x.DateTimeOfVisitLocationPoint <= visitedLocationDto.EndDateTime);

        return result.Skip(visitedLocationDto.From).Take(visitedLocationDto.Size);
    }

    public async Task<Domain.Models.Animal> CreateAnimalAsync(AnimalDto animalDto)
    {
        if (animalDto.AnimalTypes.ToHashSet().Count != animalDto.AnimalTypes.Length)
            throw new DuplicateAnimalTypeException();

        foreach (var type in animalDto.AnimalTypes)
            if (await _animalTypeRepository.ExistAsync(type) is false)
                throw new AnimalTypeNotFoundException(type);

        if (await _accountRepository.ExistAsync(animalDto.ChipperId) is false)
            throw new AccountNotFoundException(animalDto.ChipperId);

        if (await _locationPointRepository.ExistAsync(animalDto.ChippingLocationId) is false)
            throw new LocationPointNotFoundException(animalDto.ChippingLocationId);

        var animal = new Domain.Models.Animal
        {
            ChipperId = animalDto.ChipperId,
            Weight = animalDto.Weight,
            Height = animalDto.Height,
            Length = animalDto.Length,
            LifeStatus = LifeStatusConstants.Alive,
            ChippingDateTime = DateTime.Now,
            ChippingLocationId = animalDto.ChippingLocationId,
            Gender = animalDto.Gender
        };

        foreach (var typeId in animalDto.AnimalTypes) animal.Types.Add((await _animalTypeRepository.GetAsync(typeId))!);

        return await _animalRepository.CreateAsync(animal);
    }
}