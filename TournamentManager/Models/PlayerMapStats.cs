namespace TournamentManager.Models;

public class PlayerMapStats
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Kills { get; set; }
    public int Assists { get; set; }
    public int Deaths { get; set; }
    
    // Foreign keys
    public Guid PlayerId { get; set; }
    public Guid MapId { get; set; }
    
    // EF navigation 
    public virtual Player Player { get; set; } = null!;
    public virtual Map Map { get; set; } = null!;
}