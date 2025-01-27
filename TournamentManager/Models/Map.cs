namespace TournamentManager.Models;

public class Map
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public bool CompetitivePool { get; set; } // Is map in competitive pool
    public Byte[]? Image { get; set; }
    
    // EF navigation
    public virtual Game Game { get; set; } = null!;
}