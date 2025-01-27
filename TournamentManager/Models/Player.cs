namespace TournamentManager.Models;

public class Player
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nickname { get; set; } = string.Empty;
    public string Status { get; set; } = "Active"; // Current player status: 'Active', 'Benched', 'Retired'
    public string? Name { get; set; } 
    public string? Nationality { get; set; }
    public DateTime? Birthday { get; set; }
    public Byte[]? Image { get; set; }
    
    // Foreign keys
    public Guid? TeamId { get; set; }
    
    // EF navigation
    public virtual Team CurrentTeam { get; set; } = null!;
    public virtual ICollection<Team> Teams { get; set; } = null!;
}