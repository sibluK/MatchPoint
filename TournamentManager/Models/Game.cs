namespace TournamentManager.Models;

public class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int GameNumber { get; set; } // Which game in the series if bo3, bo5
    public string? GameStats { get; set; } // JSON string containing various game statistics
    
    // Foreign keys
    public Guid? WinnerId { get; set; }
    public Guid MapId { get; set; }
    public Guid MatchId { get; set; }
    public Guid Team1Id { get; set; }
    public Guid Team2Id { get; set; }
    
    // EF navigation
    public virtual Team WinnerTeam { get; set; } = null!;
    public virtual Map Map { get; set; } = null!;
    public virtual Match Match { get; set; } = null!;
    public virtual Team Team1 { get; set; } = null!;
    public virtual Team Team2 { get; set; } = null!;
}