using ItPlanet.Domain.Dto;

namespace ItPlanet.Infrastructure.Services.AnimalType;

public interface IAnimalTypeService
{
    Task<Domain.Models.AnimalType> GetAnimalTypeAsync(long id);
    Task<Domain.Models.AnimalType> CreateTypeAsync(AnimalTypeDto typeDto);
}