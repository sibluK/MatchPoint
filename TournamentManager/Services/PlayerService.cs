using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.DTOs;
using TournamentManager.Models;

namespace TournamentManager.Services;

public class PlayerService
{
    private readonly ApplicationDbContext _dbContext;
    
    public PlayerService(ApplicationDbContext applicationDbContext)
    {
        _dbContext = applicationDbContext;
    }
    
    public async Task<List<Player>> GetAllPlayersAsync()
    {
        return await _dbContext.Players.ToListAsync();
    }

    public async Task<Player> GetPlayerAndTeamByNicknameAsync(string nickname)
    {
        return await _dbContext.Players
            .Include(p => p.Team)
            .FirstAsync(p => p.Nickname == nickname);
    }

    public async Task<PlayerStatistics> GetPlayerStatsByNicknameAsync(string nickname)
    {
        var stats = new PlayerStatistics();
        
        var player = await _dbContext.Players
            .FirstAsync(p => p.Nickname == nickname);
        
        List<PlayerGameStats> playerGameStats = await _dbContext.PlayerGameStats
            .Where(p => p.PlayerId == player.Id)
            .ToListAsync();
        
        stats.MapsPlayed = playerGameStats.Count;
        stats.Kills = playerGameStats.Select(p => p.Kills).Sum();
        stats.Deaths = playerGameStats.Select(p => p.Deaths).Sum();
        stats.Assists = playerGameStats.Select(p => p.Assists).Sum();
        double totalDeaths = playerGameStats.Sum(p => (double)p.Deaths);
        stats.KDR = totalDeaths > 0 
            ? Math.Round(playerGameStats.Sum(p => (double)p.Kills) / totalDeaths, 2) 
            : Math.Round(playerGameStats.Sum(p => (double)p.Kills), 2);
        stats.KillDeathDiff = playerGameStats.Select(p => p.Kills).Sum() - playerGameStats.Select(p => p.Deaths).Sum();

        return stats;
    }
    
}