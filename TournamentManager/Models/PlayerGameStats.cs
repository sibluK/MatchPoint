namespace TournamentManager.Models;

public class PlayerGameStats
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Kills { get; set; }
    public int Assists { get; set; }
    public int Deaths { get; set; }
    
    // Foreign keys
    public Guid PlayerId { get; set; }
    public Guid GameId { get; set; }
    public Guid TeamId { get; set; }
    
    // EF navigation 
    public virtual Player Player { get; set; } = null!;
    public virtual Game Game { get; set; } = null!;
    public virtual Team Team { get; set; } = null!;
}