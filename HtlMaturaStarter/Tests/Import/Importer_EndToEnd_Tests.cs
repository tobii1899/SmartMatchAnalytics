using System;
using System.IO;
using System.Linq;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using WebApi;
using Xunit;

public class Importer_EndToEnd_Tests
{
    [Fact]
    public void Import_RealVsBarca_Writes_All_Data_To_Db()
    {
        var db = TestDbContextFactory.Create();
        var importer = new MatchImporter(db);

        importer.ImportFolder("TestData");

        Assert.Equal(1, db.Matches.Count());
        Assert.Equal(13, db.Players.Count());
        Assert.Equal(13, db.MatchEvents.Count());

        var match = db.Matches.First();
        Assert.Equal("Real", match.HomeTeam);
        Assert.Equal("Barca", match.AwayTeam);
        Assert.Equal(90, match.MatchDuration);
    }
}
