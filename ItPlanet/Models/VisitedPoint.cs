namespace ItPlanet.Models;

public class VisitedPoint
{
    public long Id { get; set; }

    public long AnimalId { get; set; }

    public long LocationPointId { get; set; }

    public DateTime DateTimeOfVisitLocationPoint { get; set; }

    public virtual Animal Animal { get; set; } = null!;

    public virtual LocationPoint LocationPoint { get; set; } = null!;
}