namespace TournamentManager.DTOs;

public class PlayerStatistics
{
    public int MapsPlayed { get; set; }
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public int Assists { get; set; }
    public double KDR { get; set; }
    public int KillDeathDiff { get; set; }
}