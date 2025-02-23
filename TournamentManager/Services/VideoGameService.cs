using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models;

namespace TournamentManager.Services;

public class VideoGameService
{
    private readonly ApplicationDbContext _dbContext;

    public VideoGameService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<VideoGame>> GetAllVideoGamesAsync()
    {
        return await _dbContext.VideoGames.ToListAsync();
    }
}