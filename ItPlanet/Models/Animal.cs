namespace ItPlanet.Models;

public class Animal
{
    public long Id { get; set; }

    public double Weight { get; set; }

    public double Length { get; set; }

    public double Height { get; set; }

    public string Gender { get; set; } = null!;

    public string LifeStatus { get; set; } = null!;

    public DateTime ChippingDateTime { get; set; }

    public int ChipperId { get; set; }

    public int ChippingLocationId { get; set; }

    public DateTime? DeathDateTime { get; set; }

    public virtual ICollection<VisitedPoint> VisitedPoints { get; } = new List<VisitedPoint>();

    public virtual ICollection<AnimalType> Types { get; } = new List<AnimalType>();
}