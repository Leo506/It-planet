using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Models;
using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.Animal;

namespace ItPlanet.Infrastructure.Services.Animal;

public class AnimalService : IAnimalService
{
    private readonly IAnimalRepository _animalRepository;

    public AnimalService(IAnimalRepository animalRepository)
    {
        _animalRepository = animalRepository;
    }

    public async Task<Domain.Models.Animal> GetAnimalAsync(long id)
    {
        var animal = await _animalRepository.GetByIdAsync(id);
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
}