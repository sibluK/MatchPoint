using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.DTOs;
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

    public async Task<Team> GetTeamByNameAsync(string name, CancellationToken ct)
    {
        try
        {
            return await _dbContext.Teams
                .Include(t => t.Players)
                .FirstOrDefaultAsync(x => x.Name == name, ct);
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

    public async Task<TeamStats> GetTeamStatsByIdAsync(Guid id, CancellationToken ct)
    {
        var stats = new TeamStats();
        var team = _dbContext.Teams.FirstOrDefault(x => x.Id == id);
        var games = new List<Game>();
        var roundCount = 0;
        var threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);
        
        var matches = await _dbContext.Matches
            .Where(m => m.Team1 == team || m.Team2 == team)
            .ToListAsync(ct);
        
        foreach (var match in matches)
        {
            var matchGames = await _dbContext.Games.Where(g => g.MatchId == match.Id).ToListAsync(ct);
            games.AddRange(matchGames);
        }

        foreach (var game in games)
        {
            roundCount += game.Team1Score + game.Team2Score;
        }
        
        stats.MatchesPlayed = matches.Count;
        stats.RoundsPlayed = roundCount;
        stats.GamesPlayed = games.Count;
        stats.Wins = matches.Count(m => m.WinnerTeamId == team.Id && m.StartDate > threeMonthsAgo);
        stats.Losses = matches.Count(m => m.StartDate > threeMonthsAgo) - stats.Wins;

        return stats;
    }
    
    public async Task CreateTeamAsync(Team team, CancellationToken ct)
    {
        Console.WriteLine($"Creating player...{team.Id}, {team.Name}");

        try
        {
            await _dbContext.Teams.AddAsync(team, ct);
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation cancelled.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating team: {ex.Message}");
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