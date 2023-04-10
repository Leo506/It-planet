using System.Text.Json.Serialization;
using ItPlanet.Domain.Geometry;

namespace ItPlanet.Domain.Models;

public class LocationPoint
{
    public long Id { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    [JsonIgnore] public virtual ICollection<Animal> Animals { get; } = new List<Animal>();

    [JsonIgnore] public virtual ICollection<VisitedPoint> VisitedPoints { get; } = new List<VisitedPoint>();

    public Point ToPoint()
    {
        return new Point(Latitude, Longitude);
    }
}