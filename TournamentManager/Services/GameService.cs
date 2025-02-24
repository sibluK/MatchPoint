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
            .ToListAsync();
    }
}