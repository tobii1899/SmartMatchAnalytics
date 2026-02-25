public class PlayerDto
{
    public string Name { get; set; }
    public string Team { get; set; }
    public int PlayedMinutes { get; set; }
    public int? ImpactScore{get;set;}=null;
}