using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models;
using TournamentManager.Models.Enums;

namespace TournamentManager.Services;

public class GameService
{
    private readonly ApplicationDbContext _dbContext;
    
    public GameService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Game>> GetMatchGames(Guid matchId)
    {
        return await _dbContext.Games
            .Include(g => g.Map)
            .Where(g => g.MatchId == matchId)
            .OrderBy(g => g.GameNumber)
            .ToListAsync();
    }

public async Task EditGames(List<Game> games)
{
    foreach (var game in games)
    {
        var existingGame = await _dbContext.Games
            .Include(g => g.Match)
                .ThenInclude(m => m.Bracket)
            .FirstOrDefaultAsync(g => g.Id == game.Id);

        if (existingGame == null) continue;

        existingGame.WinnerTeamId = game.WinnerTeamId;
        var match = existingGame.Match;

        if (match.BestOf == BestOf.BO1)
        {
            match.WinnerTeamId = game.WinnerTeamId;
            await PushWinnerToNextRound(match);
        }
        else
        {
            var matchGames = await _dbContext.Games
                .Where(g => g.MatchId == match.Id && g.WinnerTeamId != null)
                .ToListAsync();

            var team1Wins = matchGames.Count(g => g.WinnerTeamId == match.Team1Id);
            var team2Wins = matchGames.Count(g => g.WinnerTeamId == match.Team2Id);
            int requiredWins = (int)Math.Ceiling((int)match.BestOf / 2.0);

            if (team1Wins >= requiredWins)
            {
                match.WinnerTeamId = match.Team1Id;
                match.Status = ActivityStatus.Ended;
                await PushWinnerToNextRound(match);
            }
            else if (team2Wins >= requiredWins)
            {
                match.WinnerTeamId = match.Team2Id;
                match.Status = ActivityStatus.Ended;
                await PushWinnerToNextRound(match);
            }
        }
    }

    await _dbContext.SaveChangesAsync();
}

private async Task PushWinnerToNextRound(Match match)
{
    var bracket = match.Bracket;
    if (bracket == null)
    {
        Console.WriteLine($"Error: Bracket is null for match {match.Id}");
        return;
    }

    var nextRound = match.BracketRound + 1;

    var nextMatch = await _dbContext.Matches
        .Where(m => m.BracketId == bracket.Id && m.BracketRound == nextRound)
        .OrderBy(m => m.MatchNumber)
        .FirstOrDefaultAsync(m => m.MatchNumber == (match.MatchNumber + 1) / 2);

    if (nextMatch == null)
    {
        return;
    }
    
    if (nextMatch.Team1Id == match.WinnerTeamId || nextMatch.Team2Id == match.WinnerTeamId)
    {
        Console.WriteLine($"Skipping duplicate assignment for team {match.WinnerTeamId} in match {nextMatch.Id}");
        return;
    }
    
    if (nextMatch.Team1Id == null)
    {
        nextMatch.Team1Id = match.WinnerTeamId;
    }
    else if (nextMatch.Team2Id == null)
    {
        nextMatch.Team2Id = match.WinnerTeamId;
    }

    await _dbContext.SaveChangesAsync();
}

    public async Task ResetGames(List<Game> games)
    {
        foreach (var game in games)
        {
            var existingGame = await _dbContext.Games.FindAsync(game.Id);
            if (existingGame != null)
            {
                existingGame.MapId = null;
                existingGame.Team1Score = 0;
                existingGame.Team2Score = 0;
                existingGame.WinnerTeamId = null;
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}