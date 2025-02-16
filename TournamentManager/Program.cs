using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using TournamentManager.Components;
using TournamentManager.Components.Account;
using TournamentManager.Data;
using TournamentManager.Services;
using Microsoft.AspNetCore.Authentication.Google;
using TournamentManager.AutoDbUpdates;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<TournamentService>();
builder.Services.AddScoped<MatchService>();
builder.Services.AddScoped<TeamService>();
builder.Services.AddScoped<GameService>();

builder.Services.AddHostedService<TournamentStatusUpdater>();
builder.Services.AddHostedService<MatchStatusUpdater>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddLocalization();
builder.Services.AddControllers();


var app = builder.Build();

app.UseStaticFiles();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await ApplicationDbInitializer.SeedRolesAndAdmin(services);
}

string[] supportedCultures = [ "en-US", "lt-LT" ];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();
app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

/*
static void SeedData(ApplicationDbContext context)
{
    if (!context.Tournaments.Any()) // Prevents duplicate data
    {
        using var transaction = context.Database.BeginTransaction();
        
        var matches = context.Matches
            .Include(m => m.Team1)
                .ThenInclude(t => t.Players)
            .Include(m => m.Team2)
                .ThenInclude(t => t.Players)
            .ToList();

        foreach (var match in matches)
        {
            var team1 = match.Team1;
            var team2 = match.Team2;
            var games = context.Games.Where(g => g.MatchId == match.Id).ToList();

            if (!team1.Players.Any() || !team2.Players.Any())
                continue;
            
            foreach (var game in games)
            {
                foreach (var player in team1.Players)
                {
                    // Ensure player and game exist in respective tables
                    if (context.Players.Any(p => p.Id == player.Id) && context.Games.Any(g => g.Id == game.Id))
                    {
                        var stats = new PlayerGameStats { PlayerId = player.Id, GameId = game.Id };
                        context.PlayerGameStats.Add(stats);
                    }
                    else
                    {
                        // Handle missing player or game
                        Console.WriteLine($"Missing player or game: PlayerId = {player.Id}, GameId = {game.Id}");
                    }
                }
                
                foreach (var player in team2.Players)
                {
                    // Ensure player and game exist in respective tables
                    if (context.Players.Any(p => p.Id == player.Id) && context.Games.Any(g => g.Id == game.Id))
                    {
                        var stats = new PlayerGameStats { PlayerId = player.Id, GameId = game.Id };
                        context.PlayerGameStats.Add(stats);
                    }
                    else
                    {
                        // Handle missing player or game
                        Console.WriteLine($"Missing player or game: PlayerId = {player.Id}, GameId = {game.Id}");
                    }
                }
            }
        }
        
        context.SaveChanges();
        transaction.Commit();
        
        context.PlayerGameStats.AddRange(new List<PlayerGameStats>
        {
            /*
            new Player() { Nickname = "", Name = " ", Nationality = " ", Birthday = new DateTime() ,TeamId = new Guid(""), ImagePath = Path.Combine("Images", "Player", "spirit.webp").Replace("\\", "/")},
            new Player() { Nickname = "", Name = " ", Nationality = " ", Birthday = new DateTime() ,TeamId = new Guid(""), ImagePath = Path.Combine("Images", "Player", "spirit.webp").Replace("\\", "/")},
            new Player() { Nickname = "", Name = " ", Nationality = " ", Birthday = new DateTime() ,TeamId = new Guid(""), ImagePath = Path.Combine("Images", "Player", "spirit.webp").Replace("\\", "/")},
            new Player() { Nickname = "", Name = " ", Nationality = " ", Birthday = new DateTime() ,TeamId = new Guid(""), ImagePath = Path.Combine("Images", "Player", "spirit.webp").Replace("\\", "/")},
            new Player() { Nickname = "", Name = " ", Nationality = " ", Birthday = new DateTime() ,TeamId = new Guid(""), ImagePath = Path.Combine("Images", "Player", "spirit.webp").Replace("\\", "/")},
        });
        
        //context.SaveChanges();
        
        var createdTournament = context.Tournaments.FirstOrDefault(t => t.Name == "IEM Katowice 2025");
        var teams = context.Teams.ToList();
        foreach (var team in teams)
        {
            createdTournament.Teams.Add(team);
        }
        context.SaveChanges();
        
        context.Brackets.AddRange( new List<Bracket>
        {
            new Bracket() { NumberOfRounds = (int)Math.Log(16, 2), Type = BracketType.SE, TournamentId = createdTournament.Id}
        });
        
        context.SaveChanges();
        
        var createdBracket = context.Brackets.FirstOrDefault(b => b.TournamentId == createdTournament.Id);
        
        context.Matches.AddRange( new List<Match>
        {
            new Match() { StartDate = new DateTime(2025, 3, 1, 14, 0, 0), BestOf = BestOf.B01, BracketRound = 1, MatchNumber = 1, Status = ActivityStatus.Upcoming, BracketId = createdBracket.Id, Team1Id = new Guid("0A17F0BC-0083-4D9B-8E55-2EF633EEAC5A"), Team2Id = new Guid("2951CC0A-E1F8-4FDB-8516-B0331C826426") },
            new Match() { StartDate = new DateTime(2025, 3, 1, 15, 0, 0), BestOf = BestOf.B01, BracketRound = 1, MatchNumber = 2, Status = ActivityStatus.Upcoming, BracketId = createdBracket.Id, Team1Id = new Guid("3957FA6A-BD9C-409C-A784-567A79C523F7"), Team2Id = new Guid("41AD58BF-7F99-43C7-8CBA-51C32916869A")},
            
            new Match() { StartDate = new DateTime(2025, 3, 1, 16, 0, 0), BestOf = BestOf.B01, BracketRound = 1, MatchNumber = 3, Status = ActivityStatus.Upcoming, BracketId = createdBracket.Id, Team1Id = new Guid("4A65D4D8-C616-4F88-851D-3098D536A4D6"), Team2Id = new Guid("558FD2FB-22F4-47A7-A750-8F85EC4F9540")},
            new Match() { StartDate = new DateTime(2025, 3, 1, 17, 0, 0), BestOf = BestOf.B01, BracketRound = 1, MatchNumber = 4, Status = ActivityStatus.Upcoming, BracketId = createdBracket.Id, Team1Id = new Guid("60E4DB4E-A80C-4CB1-A613-9EEE73C8A965"), Team2Id = new Guid("67547350-54DF-4F65-AF4F-05F11ABADBEB")},
            
            new Match() { StartDate = new DateTime(2025, 3, 1, 18, 0, 0), BestOf = BestOf.B01, BracketRound = 1, MatchNumber = 5, Status = ActivityStatus.Upcoming, BracketId = createdBracket.Id, Team1Id = new Guid("710A2542-A40B-4DE2-831E-3DACED359E8E"), Team2Id = new Guid("7A0BC13C-D36D-4247-AF64-92C8D5A8A232")},
            new Match() { StartDate = new DateTime(2025, 3, 1, 19, 0, 0), BestOf = BestOf.B01, BracketRound = 1, MatchNumber = 6, Status = ActivityStatus.Upcoming, BracketId = createdBracket.Id, Team1Id = new Guid("7B09386D-3538-4F35-BECC-B12C887A3546"), Team2Id = new Guid("83951FDC-4A93-42D6-8DC7-139BA0A22C72")},
            
            new Match() { StartDate = new DateTime(2025, 3, 1, 20, 0, 0), BestOf = BestOf.B01, BracketRound = 1, MatchNumber = 7, Status = ActivityStatus.Upcoming, BracketId = createdBracket.Id, Team1Id = new Guid("8CCDF1B7-30D2-4CEA-8B14-4C349ECE4F9C"), Team2Id = new Guid("97433830-09AB-4699-B4FD-A53EC2B8A39C")},
            new Match() { StartDate = new DateTime(2025, 3, 1, 21, 0, 0), BestOf = BestOf.B01, BracketRound = 1, MatchNumber = 8, Status = ActivityStatus.Upcoming, BracketId = createdBracket.Id, Team1Id = new Guid("ABCF178D-16FE-4E36-B0C4-B94015A29092"), Team2Id = new Guid("AD0524CC-D33E-451F-8312-663E82834865")},
        });
        
        context.SaveChanges();
        
        var createdMatches = context.Matches.Where(m => m.BracketId == createdBracket.Id).ToList();

        foreach (var match in createdMatches)
        {
            var newGame = new Game()
                { GameNumber = 1, MapId = new Guid("14ACE175-E3A3-4383-85C0-67FB2CCD5FB5"), MatchId = match.Id };

            context.Add(newGame);
        }
        
        
        //context.SaveChanges();
        //transaction.Commit();
    }
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    context.Database.Migrate();

    SeedData(context);
}
*/


app.Run();