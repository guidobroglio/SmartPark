using Microsoft.Extensions.Logging;

namespace SmartParkManager.Gestori;

public class GestorePrenotazioni : IGestorePrenotazioni
{
    public const int MAXPOSTI = 4;
    readonly IRepositoryPrenotazione _repositoryPrenotazione;
    readonly ILogger<GestorePrenotazioni> _logger;

    public GestorePrenotazioni(IRepositoryPrenotazione repositoryPrenotazione, ILogger<GestorePrenotazioni> logger)
    {
        _repositoryPrenotazione = repositoryPrenotazione;
        _logger = logger;   
    }

    public async Task<int> InserisciPrenotazione(Prenotazione prenotazione)
    {
        DateTime oraAttuale = DateTime.Now;
        // verifica il numero dei posti già prenotati
        int postiPrenotati = await _repositoryPrenotazione.RichiediPostiPrenotati(oraAttuale);
        if(postiPrenotati < MAXPOSTI)
        {
            // inserisce la nuova prenotazione
            await _repositoryPrenotazione.InserisciPrenotazione(prenotazione);
            _logger.LogInformation("Gestore prenotazioni: inserita nuova prenotazione");
            return 0;
        }
        _logger.LogInformation("Gestore prenotazioni: impossibile inserire prenotazione, assenza di posti disponibili");
        return 1;
    }

    public async Task<List<Prenotazione>> RichiediPrenotazioniInSospeso(Guid id)
    {
        var prenotazioni = await _repositoryPrenotazione.RichiediPrenotazioniInSospeso(id);
        _logger.LogInformation($"Gestore prenotazioni: trovate {prenotazioni.Count} prenotazioni in sospeso.");
        return prenotazioni;
    }

    public async Task<List<Prenotazione>> RichiediPrenotazioniInSospeso()
    {
        var prenotazioni = await _repositoryPrenotazione.RichiediPrenotazioniInSospeso();
        _logger.LogInformation($"Gestore prenotazioni: trovate {prenotazioni.Count} prenotazioni in sospeso.");
        return prenotazioni;
    }
}
