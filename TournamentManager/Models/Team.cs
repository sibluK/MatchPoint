using TournamentManager.Models.Enum;

namespace TournamentManager.Models;

public class Team
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int Points { get; set; } = 0;
    public int Ranking { get; set; } = 0;
    public Region Region { get; set; }
    public string? ImagePath { get; set; } = string.Empty;
    
    // EF navigation
    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>(); // Many-to-Many with Tournament

}