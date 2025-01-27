namespace TournamentManager.Models;

public class Bracket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = "Single Elimination";
    public int NumberOfRounds { get; set; } // Total rounds in the bracket
    
    // Foreign keys
    public Guid TournamentId { get; set; }

    // EF navigation
    public virtual Tournament Tournament { get; set; } = null!; // one-to-one (1 bracket has 1 tournament)
    public virtual ICollection<BracketRound> Rounds { get; set; } = new List<BracketRound>(); // one-to-many (1 bracket has many rounds)
}