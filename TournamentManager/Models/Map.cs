using System.ComponentModel.DataAnnotations;

namespace TournamentManager.Models;

public class Map
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    
    // Foreign keys
    
    // EF navigation
    public virtual Game Game { get; set; } = null!;
    
}