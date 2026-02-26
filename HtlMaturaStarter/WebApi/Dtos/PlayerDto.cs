public class PlayerDto
{
    public string Name { get; set; } = null!;
    public string Team { get; set; } = null!;
    public int PlayedMinutes { get; set; }
    public int? ImpactScore{get;set;}=null;
}