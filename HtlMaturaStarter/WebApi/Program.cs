using DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add logging from .NET Aspire
builder.AddServiceDefaults();

builder.AddSqliteDbContext<ApplicationDataContext>("sqlite-db");
builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

/*
app.MapPost("/live/event", (LiveMatchService svc, LiveEventDto dto) =>
{
    try
    {
        svc.AddEvent(dto.MatchId, dto.PlayerId, dto.Minute, dto.Action);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});
*/

app.Run();