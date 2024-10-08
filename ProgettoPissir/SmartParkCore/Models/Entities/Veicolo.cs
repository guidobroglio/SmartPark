namespace SmartParkCore.Models.Entities;

public record Veicolo
{
    public int Id { get; set; }
    public Guid IdUtente { get; set; }
    public string? Targa { get; set; }
    public int CapacitaBatteria { get; set; }
    public int PercentualeBatteria { get; set; }

}
