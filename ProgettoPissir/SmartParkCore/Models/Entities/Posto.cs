namespace SmartParkCore.Models.Entities;

public record Posto
{
    public int Id { get; set; }
    public bool Disponibile { get; set; } = true;
}
