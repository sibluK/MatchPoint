using System.Text.Json.Serialization;
using TournamentManager.Models.Enums;

namespace TournamentManager.Models;

public class Bracket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int NumberOfRounds { get; set; } // Total rounds in the bracket
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BracketType Type { get; set; }
    
    // Foreign keys
    public Guid TournamentId { get; set; }

    // EF navigation
    public virtual Tournament Tournament { get; set; } = null!; // one-to-one (1 bracket has 1 tournament)
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>(); // one-to-many (1 bracket has many matches)
}