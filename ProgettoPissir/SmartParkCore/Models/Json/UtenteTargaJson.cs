using System.Text.Json.Serialization;

namespace SmartParkCore.Models.Json;

public class UtenteTargaJson
{
    [JsonPropertyName("idUtente")]
    public Guid IdUtente { get; set; }

    [JsonPropertyName("targa")]
    public string Targa { get; set; }
}
