using Microsoft.Extensions.Logging;

namespace SmartParkManager.Gestori;

public class GestoreVeicoli : IGestoreVeicoli
{
    readonly IRepositoryVeicolo _repositoryVeicolo;
    readonly ILogger<GestoreVeicoli> _logger;

    public GestoreVeicoli(IRepositoryVeicolo repositoryVeicolo, ILogger<GestoreVeicoli> logger)
    {
        _repositoryVeicolo = repositoryVeicolo;
        _logger = logger;
    }
    
    public async Task InserisciVeicoli(List<Veicolo> veicoli)
    {
        foreach (var veicolo in veicoli)
        {
            await _repositoryVeicolo.InserisciVeicolo(veicolo);
        }
        _logger.LogInformation($"Gestore veicoli: aggiunti veicoli per l'utente {veicoli.First().IdUtente}");
    }

    public async Task<List<Veicolo>> RichiediVeicoli(Guid id)
    {
        var veicoli = await _repositoryVeicolo.RichiediVeicoli(id);

        return veicoli;        
    }

    public async Task ModificaVeicoli(List<Veicolo> veicoli)
    {
        foreach (var veicolo in veicoli)
        {
            await _repositoryVeicolo.ModificaVeicolo(veicolo);
        }
    }
}
