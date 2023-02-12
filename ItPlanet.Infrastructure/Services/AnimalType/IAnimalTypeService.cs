namespace ItPlanet.Infrastructure.Services.AnimalType;

public interface IAnimalTypeService
{
    Task<Domain.Models.AnimalType> GetAnimalTypeAsync(long id);
}