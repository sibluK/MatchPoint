namespace TournamentManager.Models;

public class VideoGame
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    
    //Foreign keys and navigation properties
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}