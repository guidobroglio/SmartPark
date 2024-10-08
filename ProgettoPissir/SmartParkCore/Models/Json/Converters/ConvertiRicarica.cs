using SmartParkCore.Models.Entities;

namespace SmartParkCore.Models.Json.Converters;

public static class ConvertiRicarica
{
    public static RicaricaJson Converti(Ricarica ricarica)
    {
        return new RicaricaJson
        {
            Id = ricarica.Id,
            IdUtente = ricarica.IdUtente,
            IdPosto = ricarica.IdPosto,
            PrezzoRicarica = ricarica.PrezzoRicarica,
            PercentualeRicarica = ricarica.PercentualeRicarica,
            Veicolo = new VeicoloJson
            {
                Id = ricarica.Veicolo.Id,
                Targa = ricarica.Veicolo.Targa,
                PercentualeBatteria = ricarica.Veicolo.PercentualeBatteria,
                CapacitaBatteria = ricarica.Veicolo.CapacitaBatteria
            },
            DataInizio = ricarica.DataInizio,
            DataFine = ricarica.DataFine
        };
    }

    public static Ricarica Converti(RicaricaJson ricaricaJson)
    {
        return new Ricarica
        {
            Id = ricaricaJson.Id,
            IdUtente = ricaricaJson.IdUtente,
            IdPosto = ricaricaJson.IdPosto,
            PrezzoRicarica = ricaricaJson.PrezzoRicarica,
            PercentualeRicarica = ricaricaJson.PercentualeRicarica,
            Veicolo = new Veicolo
            {
                Id = ricaricaJson.Veicolo.Id,
                Targa = ricaricaJson.Veicolo.Targa,
                PercentualeBatteria = ricaricaJson.Veicolo.PercentualeBatteria,
                CapacitaBatteria = ricaricaJson.Veicolo.CapacitaBatteria
            },
            DataInizio = ricaricaJson.DataInizio,
            DataFine = ricaricaJson.DataFine,
            Completata = false
        };
    }
}
