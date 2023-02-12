namespace ItPlanet.Infrastructure.Repositories.AnimalType;

public interface IAnimalTypeRepository
{
    Task<Models.AnimalType?> GetTypeAsync(long id);
}