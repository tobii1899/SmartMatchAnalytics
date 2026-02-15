using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccess;

public partial class ApplicationDataContext(DbContextOptions<ApplicationDataContext> options)
    : DbContext(options)
{
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<PlayerMatch> PlayerMatches => Set<PlayerMatch>();
    public DbSet<MatchEvent> MatchEvents => Set<MatchEvent>();
}

public class ApplicationDataContextFactory : IDesignTimeDbContextFactory<ApplicationDataContext>
{
    public ApplicationDataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDataContext>();

        var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
            var configuration = configurationBuilder.Build();

        optionsBuilder.UseSqlite($"Data Source={Path.Combine(configuration["Database:path"]!, configuration["Database:fileName"]!)}");
        return new ApplicationDataContext(optionsBuilder.Options);
    }
}