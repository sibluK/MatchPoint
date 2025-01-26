namespace TournamentManager.Models;

public class Player
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public DateTime Birthday { get; set; }
    
    //Current player status: 'Active', 'Benched', 'Retired'
    public string Status { get; set; } = string.Empty;
    
    // Foreign keys
    public Guid? TeamId { get; set; }
    
    // EF navigation
    public virtual Team CurrentTeam { get; set; } = null!;
    public virtual ICollection<Team> Teams { get; set; } = null!;


    
}