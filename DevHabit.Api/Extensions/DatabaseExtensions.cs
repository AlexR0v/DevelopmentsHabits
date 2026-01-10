using DevHabit.Api.Database;
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
}
