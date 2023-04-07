using System.Text.Json.Serialization;

namespace ItPlanet.Domain.Models;

public class Area
{
    [JsonIgnore] public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<AreaPoint> AreaPoints { get; } = new List<AreaPoint>();
}
