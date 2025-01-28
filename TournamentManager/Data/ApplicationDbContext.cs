using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TournamentManager.Models;
using TournamentManager.Models.Enum;
using TournamentManager.Models.Enums;

namespace TournamentManager.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Tournament> Tournaments { get; set; }
    public DbSet<VideoGame> VideoGames { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Map> Maps { get; set; }
    public DbSet<PlayerGameStats> PlayerGameStats { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Bracket> Brackets { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Convert the BracketType enum to a string in the database
        modelBuilder.Entity<Bracket>()
            .Property(b => b.Type)
            .HasConversion(
                v => v.ToString(),
                v => (BracketType)Enum.Parse(typeof(BracketType), v)
            );
        
        modelBuilder.Entity<Game>()
            .Property(g => g.Status)
            .HasConversion(
                v => v.ToString(),
                v => (ActivityStatus)Enum.Parse(typeof(ActivityStatus), v)
            );
        
        modelBuilder.Entity<Match>()
            .Property(g => g.Status)
            .HasConversion(
                v => v.ToString(),
                v => (ActivityStatus)Enum.Parse(typeof(ActivityStatus), v)
            );
        
        modelBuilder.Entity<Tournament>()
            .Property(g => g.Status)
            .HasConversion(
                v => v.ToString(),
                v => (ActivityStatus)Enum.Parse(typeof(ActivityStatus), v)
            );
        
        modelBuilder.Entity<Player>()
            .Property(g => g.Status)
            .HasConversion(
                v => v.ToString(),
                v => (PlayerStatus)Enum.Parse(typeof(PlayerStatus), v)
            );
        
        modelBuilder.Entity<Team>()
            .Property(g => g.Region)
            .HasConversion(
                v => v.ToString(),
                v => (Region)Enum.Parse(typeof(Region), v)
            );
        
        
    }
    
    
    
}