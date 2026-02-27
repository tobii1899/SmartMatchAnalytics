using DataAccess;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

public static class MatchEndpoints
{
    public static IEndpointRouteBuilder MapMatchEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1").WithTags("Matches");

        group.MapPost("/matches", async (ApplicationDataContext context, ImportMatchRequest request ) =>
        {
            try
            {
                Console.WriteLine("TESTTEST");
                var importer = new MatchImporter(context);
                await importer.ImportMatch(request.Path);
                await context.SaveChangesAsync();

                return Results.Ok(new { message = "Match imported successfully" });
            }
            catch(Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            
        }).Produces(StatusCodes.Status200OK).Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/matches", async (ApplicationDataContext context) =>{ 
            var matches = await context.Matches.ToListAsync();
            return matches.Select(m => new MatchDto
            {
                id = m.Id,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                MatchDuration = m.MatchDuration
            }).ToList();
        }).Produces<List<MatchDto>>(StatusCodes.Status200OK);

        group.MapGet("/match/detail/{id}", ProduceMatchDetails)
        .Produces<MatchDetailsDto>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);

        group.MapGet("/match/detailscore/{id}", async (ApplicationDataContext context, int id) =>{
            return await ProduceMatchDetails(context, id, true);
        })
        .Produces<MatchDetailsDto>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> ProduceMatchDetails(ApplicationDataContext context, int id, bool includeScore = false){
        var match = await context.Matches.Where(m => m.Id == id).FirstAsync();
            
        if (match == null){
            return Results.NotFound();
        }
        
        var playerMatches = await context.PlayerMatches.Where(plm => plm.Match.Id == id).ToListAsync();
        var matchEvents = await context.MatchEvents.Where(e => e.Match.Id == id).ToListAsync();

        var players = new List<PlayerDto>();
        var events = new List<EventDto>();

        foreach(var playerMatch in playerMatches){
            Player player = context.Players.Where(pl => pl.Id == playerMatch.PlayerId).First();
            
            players.Add(new PlayerDto{
                Name = player.Name,
                Team = player.Team,
                PlayedMinutes = playerMatch.PlayedMinutes
            });
        };

        foreach(var eventMatch in matchEvents){
            
            events.Add(new EventDto{
                Name = eventMatch.Player.Name,
                Minute = eventMatch.Minute,
                Action = eventMatch.Action 
            });
        }            

        var matchDetails = new MatchDetailsDto{
            HomeTeam = match.HomeTeam,
            AwayTeam = match.AwayTeam,
            MatchDuration = match.MatchDuration,
            Players = players,
            Events = events
        }; 
        
        if(includeScore){
            var impactScores = new List<ImpactScoreDto>();

            foreach(var player in players){
                impactScores.Add(new ImpactScoreDto{
                    ImpactScore = ImpactScoreCalculator.Calculate(player, matchDetails),
                    Player = player
                });
            }
            
            matchDetails.ImpactScores = impactScores; 
        }

        return Results.Ok(matchDetails);
    }    
}

public record ImportMatchRequest(string Path);
