namespace ItPlanet.Domain.Models;

public partial class Role
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; } = new List<Account>();
}
