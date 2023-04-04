using System.Text.Json.Serialization;

namespace ItPlanet.Domain.Models;

public class Account
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    [JsonIgnore] public string Password { get; set; } = null!;

    [JsonIgnore] public int RoleId { get; set; }

    [JsonIgnore] public virtual ICollection<Animal> Animals { get; } = new List<Animal>();

    public virtual Role Role { get; set; } = null!;
}
