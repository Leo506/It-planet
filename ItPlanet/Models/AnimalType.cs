namespace ItPlanet.Models;

public class AnimalType
{
    public long Id { get; set; }

    public string Type { get; set; } = null!;
    
    public virtual ICollection<Animal> Animals { get; } = new List<Animal>();
}