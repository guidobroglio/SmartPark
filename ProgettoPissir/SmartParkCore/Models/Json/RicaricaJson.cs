using System.Text.Json.Serialization;

namespace SmartParkCore.Models.Json;

public class RicaricaJson
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("idUtente")]
    public Guid IdUtente { get; set; }

    [JsonPropertyName("idPosto")]
    public int IdPosto { get; set; }

    [JsonPropertyName("percentualeRicarica")]
    public int PercentualeRicarica { get; set; }

    [JsonPropertyName("prezzoRicarica")]
    public decimal PrezzoRicarica { get; set; }

    [JsonPropertyName("veicolo")]
    public VeicoloJson Veicolo { get; set; }

    [JsonPropertyName("dataInizio")]
    public DateTime DataInizio { get; set; }

    [JsonPropertyName("dataFine")]
    public DateTime DataFine { get; set; }
}
