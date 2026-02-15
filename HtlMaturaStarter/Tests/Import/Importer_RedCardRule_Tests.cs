using System;
using System.IO;
using System.Linq;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using WebApi;
using Xunit;
public class Importer_RedCardRule_Tests
{
    [Fact]
    public void Import_Throws_Error_When_Event_After_RedCard()
    {
        var db = TestDbContextFactory.Create();
        var importer = new MatchImporter(db);

        File.AppendAllText(
            "TestData/Real-Barca/events.txt",
            "\nReal|Carvajal|70|Goal"
        );

        Assert.Throws<InvalidOperationException>(() =>
            importer.ImportFolder("TestData"));
    }
}
