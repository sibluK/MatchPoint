using TournamentManager.Data;
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

                var team1 = new Team();
                var team2 = new Team();
                
                if (teams.Count != 0)
                {
                    team1 = teams[random.Next(0, numberOfMatches * 2 - minus)];
                    teams.Remove(team1);
                    minus++;
                    team2 = teams[random.Next(0, numberOfMatches * 2 - minus)];
                    teams.Remove(team2);
                    minus++;
                }
                
                var match = new Match
                {
                    StartDate = DateTime.Now.AddHours(j),
                    BestOf = j == numberOfMatches ? bestOfFinal : bestOf,
                    BracketRound = i,
                    MatchNumber = j,
                    BracketId = bracket.Id,
                    Team1 = team1,
                    Team2 = team2,
                };
                
                _dbContext.Matches.Add(match);

                for (var k = 1; k <= GetNumberOfGames(match.BestOf); k++)
                {
                    var game = new Game
                    {
                        GameNumber = k,
                        Team1Score = 0,
                        Team2Score = 0,
                        Match = match
                    };
                    _dbContext.Games.Add(game);
                }
                
                Console.WriteLine($"Team 1: {team1.Name}");
                Console.WriteLine($"Team 2: {team2.Name}");
                
            }
            await _dbContext.SaveChangesAsync(ct);
            numberOfMatches /= 2;
        }
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