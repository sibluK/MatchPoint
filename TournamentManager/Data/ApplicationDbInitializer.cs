using Microsoft.AspNetCore.Identity;

namespace TournamentManager.Data;

public static class ApplicationDbInitializer
{
    public static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        
        string adminEmail = configuration["AdminUser:Email"];
        string adminPassword = configuration["AdminUser:Password"];
        
        if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
        {
            Console.WriteLine("Admin email or password is missing in user secrets!");
        }
        
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
        
        var existingAdminUser = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdminUser == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
            };
            
            var createAdminUser = await userManager.CreateAsync(adminUser, adminPassword);

            if (createAdminUser.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                foreach (var error in createAdminUser.Errors)
                {
                    Console.WriteLine($"- {error.Description}");
                }
            }
        }
    }
}