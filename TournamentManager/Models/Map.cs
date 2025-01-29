namespace TournamentManager.Models;

public class Map
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public bool CompetitivePool { get; set; } // Is map in competitive pool
    public string? ImagePath { get; set; } = string.Empty;
    
    // Foreign keys
    public Guid? VideoGameId { get; set; } 
    
    // EF navigation
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}