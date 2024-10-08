namespace SmartParkCore.Models.Entities;

public record Sosta
{
    public Guid Id { get; set; }

    public Guid IdUtente { get; set; }

    public decimal PrezzoSosta { get; set; } = 0.0m;

    public Posto Posto { get; set; }

    public int Durata { get; set; } = 0;

    public Veicolo Veicolo { get; set; }

    public DateTime DataInizio { get; set; } = DateTime.Now;

    public DateTime DataFine { get; set; } = DateTime.Now;
}