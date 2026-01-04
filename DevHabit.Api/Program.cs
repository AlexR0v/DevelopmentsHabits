using DevHabit.Api.Database;
using DevHabit.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(o => o
    .UseNpgsql(builder.Configuration.GetConnectionString("Database"),
        nO => nO.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Application))
    .UseSnakeCaseNamingConvention());

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();
