using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models;

namespace TournamentManager.Services;

public class TournamentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly BracketService _bracketService;

    public TournamentService(ApplicationDbContext dbContext, BracketService bracketService)
    {
        _dbContext = dbContext;
        _bracketService = bracketService;
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

    public async Task<Tournament> GetTournamentByNameAsync(string name)
    {
        return await _dbContext.Tournaments
            .Include(tour => tour.Teams)
                .ThenInclude(team => team.Players)
            .Include(t => t.Bracket)
                .ThenInclude(b => b.Matches)
            .FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<Tournament> UpdateTournamentInfoAsync(Tournament tournament, CancellationToken ct)
    {
        var existingTournament = await _dbContext.Tournaments.FindAsync(tournament.Id);

        if (existingTournament == null)
        {
            return null!;
        }
        
        existingTournament.Name = tournament.Name;
        existingTournament.StartDate = tournament.StartDate;
        existingTournament.EndDate = tournament.EndDate;
        existingTournament.Prize = tournament.Prize;
        existingTournament.Type = tournament.Type;
        existingTournament.Status = tournament.Status;
        existingTournament.VideoGameId = tournament.VideoGameId;
        
        try
        {
            await _dbContext.SaveChangesAsync();
            return existingTournament;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating tournament: {ex.Message}");
        }

        return null!;
    }
    
    public async Task<string> CreateTournamentAsync(Tournament tournament, CancellationToken ct)
    {
        Console.WriteLine($"Creating tournament...: {tournament.Id}, {tournament.Name}");

        try
        {
            var existingTournament = await _dbContext.Tournaments.FirstOrDefaultAsync(t => t.Name == tournament.Name);

            if (existingTournament == null)
            {
                await _dbContext.Tournaments.AddAsync(tournament, ct);
                await _dbContext.SaveChangesAsync(ct);
                return null;
            }

            return $"Tournament with name: {tournament.Name} already exists!";

        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was canceled.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating tournament: {ex.Message}");
        }

        return null;
    }

    public async Task AddTeamsToTournamentAsync(Guid tournamentId, List<Team> teams, CancellationToken ct)
    {
        var existingTournament = await _dbContext.Tournaments.FindAsync(tournamentId);

        if (existingTournament != null && teams.Any())
        {
            try
            {
                var previousTeams = existingTournament.Teams;
                foreach (var team in teams)
                {
                    if (!previousTeams.Contains(team))
                    {
                        previousTeams.Add(team);
                    }
                }
                existingTournament.Teams = previousTeams;
                await _dbContext.SaveChangesAsync(ct);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding teams to tournament: {ex.Message}");
            }
        }
    }

    public async Task RemoveTeamFromTournament(Guid tournamentId, Guid teamId, CancellationToken ct)
    {
        var tournament = await _dbContext.Tournaments
            .Include(t => t.Teams)
            .FirstOrDefaultAsync(t => t.Id == tournamentId, ct);
        if (tournament == null)
        {
            return;
        }
        
        var team = tournament.Teams.FirstOrDefault(t => t.Id == teamId);
        if (team != null)
        {
            tournament.Teams.Remove(team);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteTournamentAsync(Guid tournamentId, CancellationToken ct)
    {
        //Find the tournament
        var existingTournament = await _dbContext.Tournaments
            .Include(t => t.Bracket)
            .FirstOrDefaultAsync(t => t.Id == tournamentId, ct);

        try
        {
            //Delete the bracket, matches, and games
            if (existingTournament != null && existingTournament.Bracket != null)
            {
                await _bracketService.DeleteBracket(existingTournament.Bracket, ct);
            }
            
            //Delete the tournament
            if (existingTournament != null)
            {
                _dbContext.Tournaments.Remove(existingTournament);
            }
            
            //Save changes
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting tournament: {ex.Message}");
        }

    }
}