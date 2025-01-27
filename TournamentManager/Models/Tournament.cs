namespace TournamentManager.Models;

public class Tournament
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    
    // Foreign keys
    public Guid VideoGameId { get; set; }
    public Guid? WinnerId { get; set; }
    public Guid BracketId { get; set; }
    
    //EF navigation
    public virtual VideoGame VideoGame { get; set; } = null!;
    public virtual Bracket Bracket { get; set; } = null!;
    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

}
