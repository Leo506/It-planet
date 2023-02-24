using System.Text.Json.Serialization;

namespace ItPlanet.Domain.Models;

public class VisitedPoint
{
    public long Id { get; set; }

    [JsonIgnore] public long AnimalId { get; set; }

    public long LocationPointId { get; set; }

    public DateTime DateTimeOfVisitLocationPoint { get; set; }

    [JsonIgnore] public virtual Animal Animal { get; set; } = null!;

    [JsonIgnore] public virtual LocationPoint LocationPoint { get; set; } = null!;
}