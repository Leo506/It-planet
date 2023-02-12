namespace ItPlanet.Database.Repositories.AnimalType;

public interface IAnimalTypeRepository
{
    Task<Models.AnimalType?> GetTypeAsync(long id);
}