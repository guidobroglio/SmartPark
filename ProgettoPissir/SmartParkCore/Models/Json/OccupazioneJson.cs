namespace SmartParkCore.Models.Json;

public class OccupazioneJson
{
    public int Id { get; set; }
    public bool Disponibile { get; set; } = true;
    public string Targa { get; set; } = "";
    public DateTime DataInizio { get; set; } = DateTime.MinValue;
}
