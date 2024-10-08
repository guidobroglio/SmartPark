using System.Text.RegularExpressions;

namespace SmartParkCore.Models.Topics;

public record Topics
{
    public static string ParcheggioOccupazione(int? n = null) => n == null ? "parcheggio/+/occupazione" : $"parcheggio/{n}/occupazione";
    public static string MWbotInizia(int? n = null) => n == null ? "mwbot/+/inizia_ricarica" : $"mwbot/{n}/inizia_ricarica";
    public static string MWbotRicaricaCompletata(int? n = null) => n == null ? "mwbot/+/ricarica_completata" : $"mwbot/{n}/ricarica_completata";
    public static string MWbotStatus() => "mwbot/status";
    public static string EstraiPostoParcheggio(string topic)
    {
        var match = Regex.Match(topic, @"parcheggio/(\d+)/occupazione");
        return match.Groups[1].Value;
    }
}
