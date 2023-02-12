namespace ItPlanet.Models;

public partial class Animal
{
    public IEnumerable<long> AnimalTypes => Types.Select(x => x.Id);

    public IEnumerable<long> VisitedLocations => VisitedPoints.Select(x => x.Id);
}