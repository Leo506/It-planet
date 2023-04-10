namespace ItPlanet.Domain.Dto;

public class AnalyticDto
{
    public long TotalQuantityAnimals { get; set; }
    
    public long TotalAnimalsArrived { get; set; }
    
    public long TotalAnimalsGone { get; set; }

    public List<AnimalAnalytic> AnimalsAnalytics { get; set; } = new();
}