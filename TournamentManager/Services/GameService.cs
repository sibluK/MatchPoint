using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models;

namespace TournamentManager.Services;

public class GameService
{
    private readonly ApplicationDbContext _dbContext;
    
    public GameService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Game>> GetMatchGames(Guid matchId)
    {
        return await _dbContext.Games
            .Include(g => g.Map)
            .Where(g => g.MatchId == matchId)
            .OrderBy(g => g.GameNumber)
            .ToListAsync();
    }

    public async Task EditGames(List<Game> games)
    {
        foreach (var game in games)
        {
            var existingGame = await _dbContext.Games.FindAsync(game.Id);
            if (existingGame != null)
            {
                existingGame.MapId = game.MapId;
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task ResetGames(List<Game> games)
    {
        foreach (var game in games)
        {
            var existingGame = await _dbContext.Games.FindAsync(game.Id);
            if (existingGame != null)
            {
                existingGame.MapId = null;
                existingGame.Team1Score = 0;
                existingGame.Team2Score = 0;
                existingGame.WinnerTeamId = null;
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}