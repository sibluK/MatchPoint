using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models.Enums;

namespace TournamentManager.AutoDbUpdates;

public class MatchStatusUpdater : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public MatchStatusUpdater(IServiceProvider serviceProvider)
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
                
                var allMatches = await dbContext.Matches.ToListAsync(stoppingToken);
                var currentTime = DateTime.Now;

                var liveMatches = allMatches
                    .Where(m => m.StartDate.ToUniversalTime() <= currentTime && m.Status != ActivityStatus.Ended)
                    .ToList();
                
                foreach (var liveMatch in liveMatches)
                {
                    liveMatch.Status = ActivityStatus.Live;
                }
                
                await dbContext.SaveChangesAsync(stoppingToken);
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}