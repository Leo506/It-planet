namespace ItPlanet.Models;

public class AnimalModel
{
    public long Id { get; set; }

    public long[] AnimalTypes { get; set; } = default!;
    
    public float Weight { get; set; }
    
    public float Length { get; set; }
    
    public float Height { get; set; }

    public string Gender { get; set; } = default!;

    public string LifeStatus { get; set; } = default!;
    
    public DateTime ChippingDateTime { get; set; }
    
    public int ChipperId { get; set; }
    
    public long ChippingLocationId { get; set; }

    public long[] VisitedLocations { get; set; } = default!;
    
    public DateTime DeathDateTime { get; set; }
}