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
        return await _dbContext.Tournaments.ToListAsync();
    }
}