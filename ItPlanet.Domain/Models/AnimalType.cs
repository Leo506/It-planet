using System.Text.Json.Serialization;

namespace ItPlanet.Domain.Models;

public class AnimalType
{
    public long Id { get; set; }

    public string Type { get; set; } = null!;

    [JsonIgnore] public virtual ICollection<Animal> Animals { get; } = new List<Animal>();
}