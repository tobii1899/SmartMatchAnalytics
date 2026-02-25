public class MatchDetailsDto
{
    public int Id { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public int MatchDuration { get; set; }

    public List<PlayerDto> Players { get; set; }
    public List<EventDto> Events { get; set; }
    public List<ImpactScoreDto>? ImpactScores{get;set;} = null; 
}