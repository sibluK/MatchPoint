namespace TournamentManager.Models;

public class Team
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int Points { get; set; } = 0;
    public int Ranking { get; set; } = 0;
    
    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

}