namespace SmartParkCore.Models.Entities;

public record Prezzo
{
    public int Id { get; set; }
    public decimal PrezzoSosta { get; set; }
    public decimal PrezzoRicarica { get; set; }
}
