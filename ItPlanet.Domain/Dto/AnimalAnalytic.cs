namespace ItPlanet.Domain.Dto;

public class AnimalAnalytic
{
    public string AnimalType { get; set; } = default!;

    public long AnimalTypeId { get; set; } = default!;
    
    public long QuantityAnimals { get; set; }
    
    public long AnimalsArrived { get; set; }
    
    public long AnimalsGone { get; set; }
}