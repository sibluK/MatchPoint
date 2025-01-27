namespace TournamentManager.Models;

public class BracketRound
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int RoundNumber { get; set; }
    
    // Foreign keys
    public Guid BracketId { get; set; } 
    
    // EF navigation
    public virtual Bracket Bracket { get; set; } = null!;
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
}