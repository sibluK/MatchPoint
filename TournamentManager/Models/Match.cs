namespace TournamentManager.Models;

public class Match
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime MatchDate { get; set; }
    public string Status { get; set; } = string.Empty; // Current match status: 'Upcoming', 'Ongoing', 'Finished'
    
    //Foreign keys
    public Guid? TournamentId { get; set; }
    public Guid? Team1Id { get; set; }
    public Guid? Team2Id { get; set; }
    public Guid? WinnerId { get; set; }
    
    // EF navigation
    public virtual Tournament Tournament { get; set; } = null!;
    public virtual Team Team1 { get; set; } = null!;
    public virtual Team Team2 { get; set; } = null!;
    public virtual Team WinnerTeam { get; set; } = null!;
}