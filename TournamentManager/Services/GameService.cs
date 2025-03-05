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
        //Get existing game
        var existingGame = await _dbContext.Games
            .Include(g => g.Match)
                .ThenInclude(m => m.Bracket)
                    .ThenInclude(br => br.Tournament)
            .FirstOrDefaultAsync(g => g.Id == game.Id);

        if (existingGame == null) continue;

        //Set the winner of the game if it was set
        existingGame.WinnerTeamId = game.WinnerTeamId;
        
        //Set the status to ended if a winner was set
        if (game.WinnerTeamId is not null)
        {
            existingGame.Status = ActivityStatus.Ended;
        }
        
        //Set the match that the game belongs to
        var match = existingGame.Match;
        
        // Get all games related to this match 
        var matchGames = games.Where(g => g.MatchId == match.Id && g.WinnerTeamId != null).ToList();

        // Count the wins of the teams
        var team1Wins = matchGames.Count(g => g.WinnerTeamId == match.Team1Id);
        var team2Wins = matchGames.Count(g => g.WinnerTeamId == match.Team2Id);

        //If the match was a BO1 set the winner of the match and push the winner team to the next round
        if (match.BestOf == BestOf.BO1)
        {
            match.WinnerTeamId = game.WinnerTeamId;
            match.Status = ActivityStatus.Ended;
            await PushWinnerToNextRound(match);
        }
        else
        {
            //If the match was BO3 or BO5

            var requiredWins = 0;
            
            if (match.BestOf == BestOf.BO3)
            {
                requiredWins = 2;
            }
            else if (match.BestOf == BestOf.BO5)
            {
                requiredWins = 3;
            }

            //If the team has the amount of wins needed set them as the match winner, change status to ended and push the team to the next round
            if (team1Wins == requiredWins)
            {
                match.WinnerTeamId = match.Team1Id;
                match.Status = ActivityStatus.Ended;
                await PushWinnerToNextRound(match);
            }
            else if (team2Wins == requiredWins)
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
    //Set the bracket of the match
    var bracket = match.Bracket;
    
    //Return if the bracket is null
    if (bracket == null)
    {
        Console.WriteLine($"Error: Bracket is null for match {match.Id}");
        return;
    }

    //Set the value for the next round in bracket
    var nextRound = match.BracketRound + 1;

    //This match is the final, set the tournament winner
    if (match.BracketRound == bracket.NumberOfRounds)
    {
        var tournament = await _dbContext.Tournaments.FindAsync(bracket.TournamentId);
        
        if (tournament != null)
        {
            tournament.WinnerId= match.WinnerTeamId;
            tournament.Status = ActivityStatus.Ended;
            tournament.EndDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            Console.WriteLine($"Tournament {tournament.Id} winner set to team {match.WinnerTeamId}");
        }
        else
        {
            Console.WriteLine($"Error: Tournament not found for bracket {bracket.Id}");
        }
        return;
    }

    //Find the next match in the next round to move the winner team to
    var nextMatch = await _dbContext.Matches
        .Where(m => m.BracketId == bracket.Id && m.BracketRound == nextRound)
        .OrderBy(m => m.MatchNumber)
        .FirstOrDefaultAsync(m => m.MatchNumber == (match.MatchNumber + 1) / 2);

    //Return if the next match is null
    if (nextMatch == null)
    {
        return;
    }
    
    //Check if the winner team is not already in the next round, return if so
    if (nextMatch.Team1Id == match.WinnerTeamId || nextMatch.Team2Id == match.WinnerTeamId)
    {
        Console.WriteLine($"Skipping duplicate assignment for team {match.WinnerTeamId} in match {nextMatch.Id}");
        return;
    }
    
    //Set the next match team to the winner team
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