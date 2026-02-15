using Microsoft.AspNetCore.Mvc;
using DataAccess;
using System.IO;
using WebApi.Models;

[ApiController]
[Route("api/[controller]")]
public class MatchController : ControllerBase
{
    private readonly ApplicationDataContext _db;

    public MatchController(ApplicationDataContext db)
    {
        _db = db;
    }

    [HttpPost("import-folder")]
    public IActionResult ImportFolder([FromBody] ImportFolderRequest request)
    {
        Console.WriteLine("IMPORT ENDPOINT HIT");
        try
        {
            var folderPath = request.FolderPath;
            Console.WriteLine(folderPath);

            var importer = new MatchImporter(_db);
            importer.ImportFolder(folderPath);

            return Ok("Folder imported successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("matches")]
    public IActionResult GetMatches()
    {
        var matches = _db.Matches
            .OrderByDescending(m => m.Id)
            .Select(m => new
            {
                m.Id,
                m.HomeTeam,
                m.AwayTeam,
                m.MatchDuration,
                m.IsLive
            })
            .ToList();

        return Ok(matches);
    }

    [HttpGet("match-details-with-scores/{id}")]
    public IActionResult GetMatchDetailsWithScores(int id)
    {
        var match = _db.Matches
            .Where(m => m.Id == id)
            .Select(m => new MatchDetailsDto
            {
                Id = m.Id,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                MatchDuration = m.MatchDuration,

                Players = m.PlayerMatches.Select(pm => new PlayerDto
                {
                    Name = pm.Player.Name,
                    Team = pm.Player.Team,
                    PlayedMinutes = pm.PlayedMinutes
                }).ToList(),

                Events = m.Events.Select(e => new EventDto
                {
                    Name = e.Player.Name,
                    Minute = e.Minute,
                    Action = e.Action
                }).ToList()
            })
            .FirstOrDefault();

        if (match == null)
            return NotFound("Match not found.");

        var playersWithScores = match.Players.Select(p => new
        {
            p.Name,
            p.Team,
            p.PlayedMinutes,
            Score = ImpactScoreCalculator.Calculate(p, match)
        });

        return Ok(new
        {
            match.Id,
            match.HomeTeam,
            match.AwayTeam,
            match.MatchDuration,
            Players = playersWithScores,
            match.Events
        });
    }
}