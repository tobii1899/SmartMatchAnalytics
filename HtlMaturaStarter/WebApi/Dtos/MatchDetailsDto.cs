public class MatchDetailsDto
{
    public int Id { get; set; }
    public string HomeTeam { get; set; } = null!;
    public string AwayTeam { get; set; } = null!;
    public int MatchDuration { get; set; }

    public List<PlayerDto> Players { get; set; } = null!;
    public List<EventDto> Events { get; set; } = null!;
    public List<ImpactScoreDto>? ImpactScores{get;set;} = null; 
}