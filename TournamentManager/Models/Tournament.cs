namespace TournamentManager.Models;

public class Tournament
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid? WinnerId { get; set; }
    
    // Foreign keys and navigation properties
    public Guid VideoGameId { get; set; }
    public virtual VideoGame VideoGame { get; set; } = null!;
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
}
