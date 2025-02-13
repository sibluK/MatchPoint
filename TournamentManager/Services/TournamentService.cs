using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models;

namespace TournamentManager.Services;

public class TournamentService
{
    private readonly ApplicationDbContext _dbContext;

    public TournamentService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Tournament>> GetAllTournamentsAsync()
    {
        return await _dbContext.Tournaments
            .Include(t => t.Teams)
            .OrderBy(t => t.StartDate)
            .ToListAsync();
    }

    public async Task<Tournament> GetFeaturedTournamentAsync()
    {
        return await _dbContext.Tournaments
            .Include(t => t.Teams)
            .Where(t => t.EndDate > DateTime.UtcNow)
            .OrderByDescending(t => t.Prize)
            .ThenBy(t => t.StartDate)
            .FirstOrDefaultAsync();
    }
}