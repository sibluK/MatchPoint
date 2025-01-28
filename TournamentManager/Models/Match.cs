using System.Text.Json.Serialization;
using TournamentManager.Models.Enums;

namespace TournamentManager.Models;

public class Match
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime StartDate { get; set; }
    public BestOf BestOf { get; set; }
    public int BracketRound { get; set; } // Which Round in Bracket
    public int MatchNumber { get; set; } // Order in Bracket
    public ActivityStatus Status { get; set; } = ActivityStatus.Upcoming;
    
    //Foreign keys
    public Guid? BracketId { get; set; }
    public Guid? Team1Id { get; set; }
    public Guid? Team2Id { get; set; }
    public Guid? WinnerTeamId { get; set; }
    
    // EF navigation
    public virtual Bracket Bracket { get; set; } = null!;
    public virtual Team Team1 { get; set; } = null!;
    public virtual Team Team2 { get; set; } = null!;
    public virtual Team Winner { get; set; } = null!;
}