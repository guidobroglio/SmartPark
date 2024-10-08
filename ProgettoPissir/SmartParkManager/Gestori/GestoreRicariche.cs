using Microsoft.Extensions.Logging;

namespace SmartParkManager.Gestori;

public class GestoreRicariche : IGestoreRicariche
{
    readonly IRepositoryRicarica _repositoryRicarica;
    readonly ILogger<GestoreRicariche> _logger;

    public GestoreRicariche(IRepositoryRicarica repositoryRicarica, ILogger<GestoreRicariche> logger)
    {
        _repositoryRicarica = repositoryRicarica;
        _logger = logger;
    }

    public async Task<int> RicaricheInCoda()
    {
        var ricariche = await _repositoryRicarica.RichiediRicaricheInCoda();
        _logger.LogInformation($"Gestore ricariche: trovati {ricariche} veicoli in coda per la ricarica.");

        return ricariche;
    }

    public async Task<Ricarica> RicaricaInCorso(Guid id)
    {
        var ricarica = await _repositoryRicarica.RichiediRicaricaInCorso(id);
        _logger.LogInformation($"Gestore ricariche: ricerca ricarica in corso per l'utente {id}.");

        return ricarica;
    }

    public async Task<List<Ricarica>> RicaricheUtente(Guid id)
    {
        var ricariche = await _repositoryRicarica.RichiediRicaricheUtente(id);
        _logger.LogInformation($"Gestore ricariche: trovate {ricariche.Count} ricariche per l'utente {id}.");

        return ricariche;
    }

    public async Task<List<Ricarica>> RicaricheUtenti()
    {
        var ricariche = await _repositoryRicarica.RichiediRicariche();
        _logger.LogInformation($"Gestore ricariche: trovate {ricariche.Count} ricariche.");

        return ricariche;
    }

    public async Task<Ricarica> ProssimaRicarica()
    {
        var ricarica = await _repositoryRicarica.ProssimaRicarica();

        if (ricarica != null)
        {
            _logger.LogInformation($"Gestore ricariche: veicolo {ricarica.Veicolo.Targa} in ricarica.");
        }
        else
        {
            _logger.LogInformation($"Gestore ricariche: nessun veicolo in ricarica.");
        }

        return ricarica;
    }
}
