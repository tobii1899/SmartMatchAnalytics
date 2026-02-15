using System.Collections.Generic;

public class Match
{
    public int Id { get; set; }

    public string HomeTeam { get; set; } = null!;
    public string AwayTeam { get; set; } = null!;

    public int MatchDuration { get; set; }

    public bool IsLive { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    public ICollection<PlayerMatch> PlayerMatches { get; set; } = new List<PlayerMatch>();
    public ICollection<MatchEvent> Events { get; set; } = new List<MatchEvent>();
}
