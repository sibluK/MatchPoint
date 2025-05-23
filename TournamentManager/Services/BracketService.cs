﻿using TournamentManager.Data;
using TournamentManager.Models;
using TournamentManager.Models.Enums;

namespace TournamentManager.Services;

public class BracketService
{
    private readonly ApplicationDbContext _dbContext;
    
    public BracketService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task GenerateBracket(Bracket bracket, Guid tournamentId, List<Team> teams, BestOf bestOf, BestOf bestOfFinal, CancellationToken ct)
    {
        var tournament = await _dbContext.Tournaments.FindAsync(tournamentId, ct);
        
        if (tournament != null)
        {
            if (tournament.Bracket is not null)
            {
                return;
            }
            bracket.Tournament = tournament;
            _dbContext.Brackets.Add(bracket);
            await _dbContext.SaveChangesAsync(ct);
        }
        
        var numberOfMatches = teams.Count / 2;
        // Bracket rounds creation
        for (var i = 1; i <= bracket.NumberOfRounds; i++)
        {
            Console.WriteLine($"Round: ({i})");
            
            // Matches creation
            var minus = 0;
            for (var j = 1; j <= numberOfMatches; j++)
            { 
                Console.WriteLine($"Match number in round: ({j})");
                Random random = new Random();
                
                var team1 = (Team?)null;
                var team2 = (Team?)null;
                
                if (teams.Count != 0)
                {
                    team1 = teams[random.Next(0, numberOfMatches * 2 - minus)];
                    teams.Remove(team1);
                    minus++;
                    team2 = teams[random.Next(0, numberOfMatches * 2 - minus)];
                    teams.Remove(team2);
                    minus++;
                }
                
                if (bracket == null)
                {
                    Console.WriteLine("Bracket is null!");
                    return;
                }
                if (bracket.Id == Guid.Empty)
                {
                    Console.WriteLine("BracketId is empty!");
                    return;
                }
                
                if (team1 == null || team2 == null)
                {
                    Console.WriteLine($"Error: One of the teams is null in round {i}, match {j}");
                }
                
                var match = new Match
                {
                    StartDate = DateTime.UtcNow.AddHours(3),
                    BestOf = i == bracket.NumberOfRounds ? bestOfFinal : bestOf,
                    BracketRound = i,
                    MatchNumber = j,
                    BracketId = bracket.Id,
                    Team1Id = team1 is not null ? team1.Id : null,
                    Team2Id = team2 is not null ? team2.Id : null
                };
                
                _dbContext.Matches.Add(match);
                await _dbContext.SaveChangesAsync(ct); 
                Console.WriteLine($"Match created with ID: {match.Id}");
                var savedMatch = match;
                
                for (var k = 1; k <= GetNumberOfGames(match.BestOf); k++)
                {
                    Console.WriteLine($"------------------GameNumber: ({k})------------------");
                    var game = new Game
                    {
                        GameNumber = k,
                        Team1Score = 0,
                        Team2Score = 0,
                        MatchId = savedMatch.Id
                    };
                    _dbContext.Games.Add(game);
                }

                if (team1 != null && team2 != null)
                {
                    Console.WriteLine($"Team 1: {team1.Name}");
                    Console.WriteLine($"Team 2: {team2.Name}");
                }
                else
                {
                    Console.WriteLine("No teams");
                }
                
            }
            await _dbContext.SaveChangesAsync(ct);
            numberOfMatches /= 2;
        }
    }

    public async Task DeleteBracket(Bracket bracket, CancellationToken ct)
    {
        var bracketMatches = _dbContext.Matches.Where(m => m.BracketId == bracket.Id).ToList();
        
        foreach (var match in bracketMatches)
        {
            var matchGames = _dbContext.Games.Where(g => g.MatchId == match.Id).ToList();
            _dbContext.Games.RemoveRange(matchGames);
        }
        _dbContext.Matches.RemoveRange(bracketMatches);
        _dbContext.Remove(bracket);
        await _dbContext.SaveChangesAsync(ct);
    }

    public int GetNumberOfGames(BestOf bestOf)
    {
        return bestOf switch
        {
            BestOf.BO1 => 1,
            BestOf.BO3 => 3,
            BestOf.BO5 => 5,
            _ => 1
        };
    }
}