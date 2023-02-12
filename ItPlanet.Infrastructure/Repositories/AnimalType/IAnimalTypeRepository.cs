namespace ItPlanet.Infrastructure.Repositories.AnimalType;

public interface IAnimalTypeRepository
{
    Task<Domain.Models.AnimalType?> GetTypeAsync(long id);
}