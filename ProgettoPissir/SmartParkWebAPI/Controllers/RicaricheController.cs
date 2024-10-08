using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartParkCore.Security;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartParkWebAPI.Controllers;

[ApiController]
[Route("/Api/[controller]")]
public class RicaricheController : ControllerBase
{
    readonly ILogger<RicaricheController> _logger;
    readonly IGestoreRicariche _gestoreRicariche;
    readonly IGestoreMWbot _gestoreMWbot;

    public RicaricheController(ILogger<RicaricheController> logger, IGestoreRicariche gestoreRicariche, IGestoreMWbot gestoreMWbot)
    {
        _logger = logger;
        _gestoreRicariche = gestoreRicariche;
        _gestoreMWbot = gestoreMWbot;
    }

    // GET Api/Ricariche/Completate/Utente
    [Authorize(Roles = Ruolo.Utente + "," + Ruolo.UtentePremium)]
    [HttpGet("Completate/Utente")]
    [SwaggerOperation(Summary = "Ritorna le ricariche completate",
        Description = "Questo endpoint restituisce le ricariche completate per l'utente.")]
    public async Task<List<Ricarica>> GetCompletedUserAsync(Guid id)
    {
        _logger.LogInformation("Controller ricarica: richiesta storico ricariche.");
        return await _gestoreRicariche.RicaricheUtente(id);
    }

    // GET Api/Ricariche/Completate/Amministratore
    [Authorize(Roles = Ruolo.Amministratore)]
    [HttpGet("Completate/Amministratore")]
    [SwaggerOperation(Summary = "Ritorna le ricariche completate",
        Description = "Questo endpoint restituisce le ricariche completate per tutti gli utenti.")]
    public async Task<List<Ricarica>> GetCompletedAdminAsync()
    {
        _logger.LogInformation("Controller ricarica: richiesta storico ricariche.");
        return await _gestoreRicariche.RicaricheUtenti();
    }

    // GET Api/Ricariche/InCorso
    [Authorize(Roles = Ruolo.Utente + "," + Ruolo.UtentePremium)]
    [HttpGet("InCorso")]
    [SwaggerOperation(Summary = "Ritorna le ricariche in corso",
        Description = "Questo endpoint restituisce le ricariche in corso per l'utente.")]
    public async Task<Ricarica?> GetRunningAsync(Guid id)
    {
        _logger.LogInformation("Controller ricarica: richiesta ricarica in corso.");
        return await _gestoreRicariche.RicaricaInCorso(id);
    }

    // GET Api/Ricariche/InCoda
    [HttpGet("InCoda")]
    [SwaggerOperation(Summary = "Ritorna le ricariche in coda",
        Description = "Questo endpoint restituisce tutte le ricariche in coda, ovvero i veicoli in attesa dell'MWbot.")]
    public async Task<int> GetQueuedAsync()
    {
        _logger.LogInformation("Controller ricarica: richiesta ricariche in coda.");
        return await _gestoreRicariche.RicaricheInCoda();
    }

    // GET Api/Ricariche/MWbot
    [HttpGet("MWbot")]
    [SwaggerOperation(Summary = "Ritorna la ricarica in corso dell'MWbot",
        Description = "Questo endpoint restituisce la ricarica che sta eseguendo l'MWbot.")]
    public async Task<Ricarica> GetMWbotAsync()
    {
        _logger.LogInformation("Controller ricarica: richiesta ricarica attuale MWbot.");
        return await _gestoreRicariche.ProssimaRicarica();
    }

    // POST Api/Ricariche
    [Authorize(Roles = Ruolo.Utente + "," + Ruolo.UtentePremium)]
    [HttpPost]
    [SwaggerOperation(Summary = "Inserisce una richiesta di ricarica",
        Description = "Questo endpoint permette di richiedere una ricarica, che verrà completata quando l'MWbot è libero.")]
    public async Task<int> PostAsync(Ricarica ricarica)
    {
        _logger.LogInformation($"Controller ricarica: richiesta inizio ricarica dal {ricarica.Veicolo.PercentualeBatteria}% al {ricarica.PercentualeRicarica}%.");
        return await _gestoreMWbot.RichiediRicarica(ricarica);
    }
}
