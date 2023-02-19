namespace ItPlanet.Infrastructure.Repositories.AnimalType;

public interface IAnimalTypeRepository : IRepository<Domain.Models.AnimalType, long>
{
    Task<Domain.Models.AnimalType?> GetByType(string type);
}