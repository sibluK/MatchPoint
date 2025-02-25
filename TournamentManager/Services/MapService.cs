using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models;

namespace TournamentManager.Services;

public class MapService
{
    private readonly ApplicationDbContext _dbContext;

    public MapService(ApplicationDbContext context)
    {
        _dbContext = context;
    }

    public async Task<List<Map>> GetAllMapsAsync(CancellationToken ct)
    {
        return await _dbContext.Maps.ToListAsync(ct);
    }
}