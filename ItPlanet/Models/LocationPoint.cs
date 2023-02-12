namespace ItPlanet.Models;

public class LocationPoint
{
    public long Id { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public virtual ICollection<VisitedPoint> VisitedPoints { get; } = new List<VisitedPoint>();
}