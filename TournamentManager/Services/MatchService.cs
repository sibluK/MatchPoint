using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models;
using TournamentManager.Models.Enums;

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
                        m.StartDate.Year == DateTime.Now.Year && m.Status != ActivityStatus.Ended)
            .Include(m => m.Bracket)
            .ThenInclude(b => b.Tournament)
            .Include(m => m.Team1)
            .Include(m => m.Team2)
            .ToListAsync();
    }

    public async Task EditMatchDateTime(Match match, CancellationToken ct)
    {
        var existingMatch = await _dbContext.Matches.FindAsync(match.Id, ct);

        if (existingMatch != null)
        {
            existingMatch.StartDate = match.StartDate;
        }

        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<Match> GetMatchByIdAsync(Guid matchId)
    {
        
        var match = await _dbContext.Matches
            .Include(m => m.Team1)
            .Include(m => m.Team2)
            .Include(m => m.Bracket)
            .ThenInclude(b => b.Tournament)
            .FirstOrDefaultAsync(m => m.Id == matchId);

        if (match == null)
        {
            Console.WriteLine("Match not found");
            return null;
        }

        return match;
    }
    
    public async Task<Tuple<int, int>> GetHeadToHeadScore(Guid team1Id, Guid team2Id)
    {
        var matches = await _dbContext.Matches
            .Where(m => (m.Team1Id == team1Id && m.Team2Id == team2Id) || (m.Team1Id == team2Id && m.Team2Id == team1Id))
            .Take(20)
            .ToListAsync();
        
        int team1Score = 0;
        int team2Score = 0;

        foreach (var match in matches)
        {
            if (match.WinnerTeamId != null)
            {
                if (match.WinnerTeamId == team1Id)
                {
                    team1Score++;
                }
                else
                {
                    team2Score++;
                }
            }
        }
        
        return Tuple.Create(team1Score, team2Score);
    }
    
    public async Task<int> GetMapWinrate(Guid teamId, Guid mapId)
    {
        var games = await _dbContext.Games
            .Where(g => g.MapId == mapId && 
                        (g.Match.Team1Id == teamId || g.Match.Team2Id == teamId))
            .Take(15)
            .ToListAsync();

        if (games.Count == 0)
            return 0; 

        var wins = games.Count(g => g.WinnerTeamId == teamId);

        return (int)((double)wins / games.Count * 100); 
    }
}