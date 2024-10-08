namespace SmartParkCore.Models.Entities;

public record Pagamento
{
    public Guid Id { get; set; }
    public Guid IdUtente { get; set; }
    public decimal ImportoSosta { get; set; }
    public decimal ImportoRicarica { get; set; }
    public DateTime Data { get; set; }
    required public Veicolo Veicolo { get; set; }
}
