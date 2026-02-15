public class MatchEvent
{
    public int Id { get; set; }

    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;

    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    public int Minute { get; set; }

    public string Action { get; set; } = null!;
}
