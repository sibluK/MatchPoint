using TournamentManager.Models.Enums;

namespace TournamentManager.Models;

public class Player
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nickname { get; set; } = string.Empty;
    public PlayerStatus Status { get; set; } = PlayerStatus.Active;
    public string? Name { get; set; } 
    public string? Nationality { get; set; }
    public DateTime? Birthday { get; set; }
    public string? ImagePath { get; set; } = string.Empty;
    
    // Foreign keys
    public Guid? TeamId { get; set; }
    
    // EF navigation
    public virtual Team Team { get; set; } = null!;
}