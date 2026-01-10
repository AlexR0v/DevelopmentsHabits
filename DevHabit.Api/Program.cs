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
}

app.MapControllers();

await app.RunAsync();
