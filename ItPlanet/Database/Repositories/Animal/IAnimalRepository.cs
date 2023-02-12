namespace ItPlanet.Database.Repositories.Animal;

public interface IAnimalRepository
{
    Task<Models.Animal?> GetByIdAsync(long id);
}