namespace ItPlanet.Services.AnimalType;

public interface IAnimalTypeService
{
    Task<Models.AnimalType> GetAnimalTypeAsync(long id);
}