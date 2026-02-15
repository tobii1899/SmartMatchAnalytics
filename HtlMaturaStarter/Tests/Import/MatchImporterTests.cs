using Xunit;
using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using WebApi;

namespace Tests.Import
{
    public class MatchImporterAdditionalTests
    {
        private readonly DbContextOptions<ApplicationDataContext> _options;

        public MatchImporterAdditionalTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void ImportMatch_ShouldImport_AllPlayersCorrectly_VerifyFermin()
        {
            using var context = new ApplicationDataContext(_options);
            var importer = new MatchImporter(context);

            importer.ImportMatch("TestData/Real-Barca");

            var match = context.Matches
                .Include(m => m.PlayerMatches)
                .ThenInclude(pm => pm.Player)
                .First();

            var fermin = match.PlayerMatches
                .FirstOrDefault(pm => pm.Player.Name == "Fermin Lopez");

            Assert.NotNull(fermin);
            Assert.Equal("Barca", fermin.Player.Team);
            Assert.Equal(90, fermin.PlayedMinutes);
        }

        [Fact]
        public void ImportMatch_ShouldAssign_EventsToCorrectPlayers()
        {
            using var context = new ApplicationDataContext(_options);
            var importer = new MatchImporter(context);

            importer.ImportMatch("TestData/Real-Barca");

            var match = context.Matches
                .Include(m => m.Events)
                .ThenInclude(e => e.Player)
                .First();

            var ferminGoals = match.Events
                .Where(e => e.Player.Name == "Fermin Lopez" && e.Action == "Goal");

            Assert.Equal(4, ferminGoals.Count());
        }

        [Fact]
        public void ImportMatch_ShouldNotContain_InvalidMinutes()
        {
            using var context = new ApplicationDataContext(_options);
            var importer = new MatchImporter(context);

            importer.ImportMatch("TestData/Real-Barca");

            var match = context.Matches
                .Include(m => m.Events)
                .First();

            Assert.DoesNotContain(match.Events, e => e.Minute > match.MatchDuration);
        }

        [Fact]
        public void ImportMatch_ShouldParse_TeamsFromFolderName()
        {
            using var context = new ApplicationDataContext(_options);
            var importer = new MatchImporter(context);

            importer.ImportMatch("TestData/Real-Barca");

            var match = context.Matches.First();

            Assert.Equal("Real", match.HomeTeam);
            Assert.Equal("Barca", match.AwayTeam);
        }
    }
}
