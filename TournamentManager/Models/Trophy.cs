namespace TournamentManager.Models;

public class Trophy
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TeamId { get; set; }
    public Guid? TournamentId { get; set; }
}