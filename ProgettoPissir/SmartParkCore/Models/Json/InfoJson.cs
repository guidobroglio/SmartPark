namespace SmartParkCore.Models.Json;

public class InfoJson
{
    public Prezzo Prezzo { get; set; }
    public IList<OccupazioneJson> Occupazioni { get; set; }
}
