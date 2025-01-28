using System.Text.Json.Serialization;
using TournamentManager.Models.Enums;

namespace TournamentManager.Models;

public class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int GameNumber { get; set; } // Which game in match if bo3, bo5
    public string? GameStats { get; set; } // JSON string containing various game statistics
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ActivityStatus Status { get; set; } = ActivityStatus.Upcoming;
    
    // Foreign keys
    public Guid? WinnerTeamId { get; set; }
    public Guid MapId { get; set; }
    public Guid MatchId { get; set; }
    
    // EF navigation
    public virtual Team WinnerTeam { get; set; } = null!;
    public virtual Map Map { get; set; } = null!;
    public virtual Match Match { get; set; } = null!;
    public virtual ICollection<PlayerGameStats> PlayerGameStats { get; set; } = new List<PlayerGameStats>();
}
