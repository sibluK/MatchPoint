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
    
    public async Task<List<Player>> GetAllPlayersAsync(CancellationToken ct)
    {
        try
        {
            return await _dbContext.Players.ToListAsync(ct);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation cancelled.");
            return null!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching player: {ex.Message}");
            return null!;
        }
    }

    public async Task<Player> GetPlayerAndTeamByNicknameAsync(string nickname, CancellationToken ct)
    {
        try
        {
            return await _dbContext.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.Nickname == nickname, ct);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was canceled.");
            return null!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching player: {ex.Message}");
            return null!;
        }
    }

    public async Task UpdatePlayerAsync(Player player)
    {
        var existingPlayer = await _dbContext.Players.FindAsync(player.Id);
    
        if (existingPlayer is null)
        {
            return;
        }
        
        existingPlayer.Name = player.Name;
        existingPlayer.Nickname = player.Nickname;
        existingPlayer.Birthday = player.Birthday;
        existingPlayer.ImagePath = player.ImagePath;
        existingPlayer.Rating = player.Rating;
        existingPlayer.Nationality = player.Nationality;
        existingPlayer.Status = player.Status;
        existingPlayer.TeamId = player.TeamId;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating player: {ex.Message}");
        }
    }
    
    public async Task<PlayerStatistics> GetPlayerStatsByNicknameAsync(string nickname, CancellationToken ct)
    {
        var stats = new PlayerStatistics();
        
        var player = await _dbContext.Players
            .FirstOrDefaultAsync(p => p.Nickname == nickname, ct);

        if (player is null)
        {
            return null!;
        }
        
        List<PlayerGameStats> playerGameStats = await _dbContext.PlayerGameStats
            .Where(p => p.PlayerId == player.Id)
            .ToListAsync(ct);
        
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