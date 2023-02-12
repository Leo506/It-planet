using System.Text.Json.Serialization;

namespace ItPlanet.Domain.Models;

public partial class LocationPoint
{
    public long Id { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    [JsonIgnore] public virtual ICollection<VisitedPoint> VisitedPoints { get; } = new List<VisitedPoint>();
}
