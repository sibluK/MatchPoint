using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models;

namespace TournamentManager.Services;

public class MatchService
{
    private readonly ApplicationDbContext _dbContext;

    public MatchService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Match>> GetAllMatchesAsync()
    {
        return await _dbContext.Matches
            .Include(m => m.Bracket)
                .ThenInclude(b => b.Tournament)
            .Include(m => m.Team1)
            .Include(m => m.Team2)
            .ToListAsync();
    }

    public async Task<List<Match>> GetTodaysMatchesAsync()
    {
        return await _dbContext.Matches
            .Where(m => m.StartDate.Day == DateTime.Now.Day && m.StartDate.Month == DateTime.Now.Month &&
                        m.StartDate.Year == DateTime.Now.Year)
            .Include(m => m.Bracket)
            .ThenInclude(b => b.Tournament)
            .Include(m => m.Team1)
            .Include(m => m.Team2)
            .ToListAsync();
    }
}