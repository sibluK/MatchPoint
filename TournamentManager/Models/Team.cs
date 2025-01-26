namespace TournamentManager.Models;

public class Team
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
     
    // Foreign keys
    
    
    // EF navigation
    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

}