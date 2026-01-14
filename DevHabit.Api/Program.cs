using DevHabit.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddControllers()
    .AddErrorHanding()
    .AddDbConnection()
    .AddValidation()
    .AddServices()
    .AddOpenTelemetry()
    .AddAuthService();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.ApplyMigrationsAsync();
    await app.SeedInitialDataAsync();
}

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
