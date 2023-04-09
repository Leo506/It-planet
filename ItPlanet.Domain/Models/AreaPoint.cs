using System.Text.Json.Serialization;

namespace ItPlanet.Domain.Models;

public class AreaPoint
{
    [JsonIgnore] public int Id { get; set; }

    [JsonIgnore] public long AreaId { get; set; }

    public double Longitude { get; set; }

    public double Latitude { get; set; }

    [JsonIgnore] public virtual Area Area { get; set; } = null!;
}
