using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<PlayerMatch> PlayerMatches => Set<PlayerMatch>();
    public DbSet<MatchEvent> MatchEvents => Set<MatchEvent>();
}
