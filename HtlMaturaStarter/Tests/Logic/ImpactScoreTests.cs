using System;
using System.IO;
using System.Linq;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using WebApi;
using Xunit;
public class ImpactScoreTests
{
    [Fact]
    public void Fermin_Lopez_Has_Highest_Score()
    {
        var db = TestDbContextFactory.Create();
        var importer = new MatchImporter(db);

        importer.ImportFolder("TestData");

        var match = db.Matches
            .Include(m => m.PlayerMatches)
            .Include(m => m.Events)
            .First();

        var fermin = match.PlayerMatches
            .First(pm => pm.Player.Name == "Fermin Lopez");

        var score = ImpactScoreCalculator.Calculate(fermin, match);

        Assert.True(score > 15);
    }

    [Fact]
    public void Player_With_Zero_Minutes_Has_Zero_Score()
    {
        var match = new Match { MatchDuration = 90 };
        var pm = new PlayerMatch { PlayedMinutes = 0 };

        var score = ImpactScoreCalculator.Calculate(pm, match);

        Assert.Equal(0, score);
    }
}
