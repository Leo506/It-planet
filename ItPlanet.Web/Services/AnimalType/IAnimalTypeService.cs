using ItPlanet.Domain.Dto;

namespace ItPlanet.Web.Services.AnimalType;

public interface IAnimalTypeService
{
    Task<Domain.Models.AnimalType> GetAnimalTypeAsync(long id);
    Task<Domain.Models.AnimalType> CreateTypeAsync(AnimalTypeDto typeDto);
    Task<Domain.Models.AnimalType> UpdateType(long typeId, AnimalTypeDto animalTypeDto);
    Task DeleteTypeAsync(long typeId);
    Task<Domain.Models.Animal> AddTypeToAnimalAsync(long animalId, Domain.Models.AnimalType type);
    Task<Domain.Models.Animal> ReplaceAnimalTypeAsync(long animalId, Domain.Models.AnimalType oldType, Domain.Models.AnimalType newType);
    Task<Domain.Models.Animal> DeleteAnimalTypeFromAnimalAsync(long animalId, Domain.Models.AnimalType type);
}