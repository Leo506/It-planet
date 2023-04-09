namespace ItPlanet.Domain.Models;

public class Area
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;
    
    public virtual ICollection<AreaPoint> AreaPoints { get; } = new List<AreaPoint>();
}
