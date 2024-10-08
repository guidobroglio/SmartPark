namespace SmartParkCore.Models.Entities;

public record Ricarica
{
    public Guid Id { get; set; }
    public Guid IdUtente { get; set; }
    public int IdPosto { get; set; }
    public decimal PrezzoRicarica { get; set; }
    public int PercentualeRicarica { get; set; }
    public Veicolo Veicolo { get; set; }
    public DateTime DataInizio { get; set; }
    public DateTime DataFine { get; set; }
    public bool Completata { get; set; }
}
