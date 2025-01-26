namespace TournamentManager.Models;

public class Team
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    
    public virtual ICollection<Trophy> Trophies { get; set; } = new List<Trophy>();
    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

}