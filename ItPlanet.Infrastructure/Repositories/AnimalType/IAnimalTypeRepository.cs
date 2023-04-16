namespace ItPlanet.Infrastructure.Repositories.AnimalType;

public interface IAnimalTypeRepository
{
    Task<Domain.Models.AnimalType?> GetAsync(long id);
    Task<Domain.Models.AnimalType> CreateAsync(Domain.Models.AnimalType model);
    Task<Domain.Models.AnimalType> UpdateAsync(Domain.Models.AnimalType model);
    Task DeleteAsync(Domain.Models.AnimalType model);
    Task<bool> ExistAsync(long id);
    Task<bool> ExistAsync(string typeName);
}