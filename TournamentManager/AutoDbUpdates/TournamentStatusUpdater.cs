using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models.Enums;

namespace TournamentManager.AutoDbUpdates;

public class TournamentStatusUpdater : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public TournamentStatusUpdater(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var currentTime = DateTime.UtcNow;
                
                var liveTournaments = await dbContext.Tournaments
                    .Where(t => t.StartDate <= currentTime && t.EndDate > currentTime)
                    .ToListAsync(stoppingToken);

                foreach (var liveTournament in liveTournaments)
                {
                    liveTournament.Status = ActivityStatus.Live;
                }
                
                var endedTournaments = await dbContext.Tournaments
                    .Where(t => t.EndDate <= currentTime)
                    .ToListAsync(stoppingToken);

                foreach (var endedTournament in endedTournaments)
                {
                    endedTournament.Status = ActivityStatus.Ended;
                }
                
                await dbContext.SaveChangesAsync(stoppingToken);
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
    
}