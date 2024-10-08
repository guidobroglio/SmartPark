namespace SmartParkCore.Models.Entities;

public record Prenotazione
{
    public Guid Id { get; set; }
    public Guid IdUtente { get; set; }
    public DateTime DataInizio { get; set; }
    public DateTime DataFine { get; set; }
    public bool Stato { get; set; }
}
