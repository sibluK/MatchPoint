using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
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
}