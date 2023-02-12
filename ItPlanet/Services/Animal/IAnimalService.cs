namespace ItPlanet.Services.Animal;

public interface IAnimalService
{
    Task<Models.Animal> GetAnimalAsync(long id);
}