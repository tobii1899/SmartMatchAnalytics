public class Player
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string Team { get; set; } = null!;

    public ICollection<PlayerMatch> PlayerMatches { get; set; } = new List<PlayerMatch>();
}
