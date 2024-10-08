using System.Text.Json.Serialization;

namespace SmartParkCore.Models.Json;

public class VeicoloJson
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("targa")]
    required public string Targa { get; set; }

    [JsonPropertyName("capacitaBatteria")]
    public int CapacitaBatteria { get; set; }

    [JsonPropertyName("percentualeBatteria")]
    public int PercentualeBatteria { get; set; }
}
