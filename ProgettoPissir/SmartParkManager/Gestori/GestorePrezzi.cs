using Microsoft.Extensions.Logging;

namespace SmartParkManager.Gestori;

public class GestorePrezzi : IGestorePrezzi
{
    readonly IRepositoryPrezzo _repositoryPrezzo;
    readonly ILogger<GestorePrezzi> _logger;

    public GestorePrezzi(IRepositoryPrezzo repositoryPrezzo, ILogger<GestorePrezzi> logger)
    {
        _repositoryPrezzo = repositoryPrezzo;
        _logger = logger;
    }

    public async Task<Prezzo> RichiediPrezzi()
    {
        _logger.LogInformation("Gestreo prezzi: Restituiti prezzi sosta e ricarica");
        return await _repositoryPrezzo.RichiediPrezzi();
    }

    public async Task ModificaPrezzi(Prezzo prezzo)
    {
        _logger.LogInformation("Gestreo prezzi: Restituiti prezzi sosta e ricarica");
        await _repositoryPrezzo.ModificaPrezzi(prezzo);
    }
}
