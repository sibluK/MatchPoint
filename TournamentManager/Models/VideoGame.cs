namespace TournamentManager.Models;

public class VideoGame
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? ImagePath { get; set; } = string.Empty;
    
    // EF navigation
    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
    public virtual ICollection<Map> Maps { get; set; } = new List<Map>();
}