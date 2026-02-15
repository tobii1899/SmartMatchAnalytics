using System;
using System.Linq;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using WebApi;
using Xunit;

namespace Tests.Import
{
    public class MatchImporterTests
    {
        private readonly DbContextOptions<ApplicationDataContext> _options;

        public MatchImporterTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDataContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;
        }

        [Fact]
        public void ImportMatch_ValidData_ShouldImportMatch()
        {
            using var context = new ApplicationDataContext(_options);
            var importer = new MatchImporter(context);

            importer.ImportMatch("TestData/Real-Barca");

            var match = context.Matches
                .Include(m => m.PlayerMatches)
                .ThenInclude(pm => pm.Player)
                .Include(m => m.Events)
                .FirstOrDefault();

            Assert.NotNull(match);
            Assert.Equal("Real", match.HomeTeam);
            Assert.Equal("Barca", match.AwayTeam);
            Assert.Equal(90, match.MatchDuration);

            // Deine Datei enthält 14 Spieler
            Assert.Equal(14, match.PlayerMatches.Count);

            // Deine events.txt enthält 14 Events
            Assert.Equal(14, match.Events.Count);
        }

        [Fact]
        public void ImportMatch_InvalidFolderName_ShouldThrowException()
        {
            using var context = new ApplicationDataContext(_options);
            var importer = new MatchImporter(context);

            Assert.Throws<Exception>(() =>
                importer.ImportMatch("InvalidFolderName"));
        }

        [Fact]
        public void ImportMatch_EventOutsideMatchDuration_ShouldThrowException()
        {
            using var context = new ApplicationDataContext(_options);
            var importer = new MatchImporter(context);

            Assert.Throws<InvalidOperationException>(() =>
                importer.ImportMatch("TestData/Real-Barca"));
        }

        [Fact]
        public void ImportMatch_EventAfterRedCard_ShouldThrowException()
        {
            using var context = new ApplicationDataContext(_options);
            var importer = new MatchImporter(context);

            Assert.Throws<InvalidOperationException>(() =>
                importer.ImportMatch("TestData/Real-Barca"));
        }

        [Fact]
        public void ImportMatch_DuplicatePlayer_ShouldThrowException()
        {
            using var context = new ApplicationDataContext(_options);
            var importer = new MatchImporter(context);

            Assert.Throws<InvalidOperationException>(() =>
                importer.ImportMatch("TestData/Real-Barca"));
        }
    }
}
