namespace TournamentManager.Models;

public class Tournament
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    
    // Foreign keys
    public Guid? WinnerId { get; set; }
    public Guid VideoGameId { get; set; }
    
    // EF navigation
    public virtual VideoGame VideoGame { get; set; } = null!;
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
}
