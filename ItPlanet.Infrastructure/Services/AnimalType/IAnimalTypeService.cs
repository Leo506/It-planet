namespace ItPlanet.Infrastructure.Services.AnimalType;

public interface IAnimalTypeService
{
    Task<Models.AnimalType> GetAnimalTypeAsync(long id);
}