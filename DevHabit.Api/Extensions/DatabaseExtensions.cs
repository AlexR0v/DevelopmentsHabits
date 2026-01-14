using DevHabit.Api.Database;
using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await using ApplicationIdentityDbContext identityContext =
            scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
        try
        {
            await context.Database.MigrateAsync();
            app.Logger.LogInformation("Database migrations apply successfully.");

            await identityContext.Database.MigrateAsync();
            app.Logger.LogInformation("Database Identity migrations apply successfully.");
        }
        catch (Exception e)
        {
            app.Logger.LogError(e, "Migrations apply failed.");
            throw;
        }
    }

    public static async Task SeedInitialDataAsync(this WebApplication app)
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        using RoleManager<IdentityRole> roleManager =
            scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        try
        {
            if (!await roleManager.RoleExistsAsync(Roles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            }

            if (!await roleManager.RoleExistsAsync(Roles.Member))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Member));
            }

            app.Logger.LogInformation("Roles created successfully");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred while seeding initial data");
            throw;
        }
    }
}
