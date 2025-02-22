using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models;

namespace TournamentManager.Services;

public class TeamService
{
    private readonly ApplicationDbContext _dbContext;

    public TeamService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Team>> GetAllTeamsAsync(CancellationToken ct)
    {
        try
        {
            return await _dbContext.Teams.ToListAsync(ct);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation cancelled.");
            return null!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching team: {ex.Message}");
            return null!;
        }
    }

    public async Task UpdateTeamAsync(Team team, CancellationToken ct)
    {
        var existingTeam = await _dbContext.Teams.FirstOrDefaultAsync(x => x.Id == team.Id, ct);

        if (existingTeam is null)
        {
            return;
        }
        
        existingTeam.Name = team.Name;
        existingTeam.Points = team.Points;
        existingTeam.Ranking = team.Ranking;
        existingTeam.Region = team.Region;
        existingTeam.ImagePath = team.ImagePath;

        try
        {
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating team: {ex.Message}");
        }
    }
}