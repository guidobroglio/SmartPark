using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartParkCore.Security;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartParkWebAPI.Controllers;

[ApiController]
[Route("/Api/[controller]")]
public class SosteController : ControllerBase
{
    readonly ILogger<SosteController> _logger;
    readonly IGestoreParcheggio _gestoreParcheggio;

    public SosteController(ILogger<SosteController> logger, IGestoreParcheggio gestoreParcheggio)
    {
        _logger = logger;
        _gestoreParcheggio = gestoreParcheggio;
    }

    // POST Api/Soste/Inserimento
    [Authorize(Roles = Ruolo.Utente + "," + Ruolo.UtentePremium)]
    [HttpPost("Inserimento")]
    [SwaggerOperation(Summary = "Inserisce una sosta",
        Description = "Questo endpoint permette di inserire una sosta per un veicolo dell'utente.")]
    public async Task<int> PostInsertAsync(Sosta sosta)
    {
        _logger.LogInformation("Controller sosta: inserimento sosta");
        return await _gestoreParcheggio.InserisciSosta(sosta);
    }

    // POST Api/Soste/Richiesta
    [Authorize(Roles = Ruolo.Utente + "," + Ruolo.UtentePremium)]
    [HttpPost("Richiesta")]
    [SwaggerOperation(Summary = "Richiede i posti disponibili",
        Description = "Questo endpoint calcola e restituisce il primo posto disponibile nel parcheggio.")]
    public async Task<int> PostRequestAsync()
    {
        _logger.LogInformation("Controller sosta: calcolo posti disponibili");
        int posto = await _gestoreParcheggio.CalcolaPosto();

        return posto;
    }

    // GET Api/Soste/Utente
    [Authorize(Roles = Ruolo.Utente + "," + Ruolo.UtentePremium)]
    [HttpGet("Utente")]
    [SwaggerOperation(Summary = "Restituisce le soste terminate",
        Description = "Questo endpoint restituisce le soste terminate dell'utente.")]

    public async Task<List<Sosta>> GetUserAsync(Guid id)
    {
        _logger.LogInformation("Controller soste: richiesta storico soste.");
        return await _gestoreParcheggio.RichiediSosteUtente(id);
    }

    // GET Api/Soste/Amministratore
    [Authorize(Roles = Ruolo.Amministratore)]
    [HttpGet("Amministratore")]
    [SwaggerOperation(Summary = "Restituisce le soste terminate",
        Description = "Questo endpoint restituisce le soste terminate di tutti gli utenti.")]
    public async Task<List<Sosta>> GetAdminAsync()
    {
        _logger.LogInformation("Controller soste: richiesta storico soste.");
        return await _gestoreParcheggio.RichiediSoste();
    }

    // GET Api/Soste/InCorso/Utente
    [Authorize(Roles = Ruolo.Utente + "," + Ruolo.UtentePremium)]
    [HttpGet("InCorso/Utente")]
    [SwaggerOperation(Summary = "Restituisce le soste in corso",
        Description = "Questo endpoint restituisce le soste in corso dell'utente.")]
    public async Task<List<Sosta>> GetStartedUserAsync(Guid id)
    {
        _logger.LogInformation("Controller soste: richiesta storico soste.");
        return await _gestoreParcheggio.RichiediSosteInCorso(id);
    }

    // GET Api/Soste/InCorso/Amministratore
    [Authorize(Roles = Ruolo.Amministratore)]
    [HttpGet("InCorso/Amministratore")]
    [SwaggerOperation(Summary = "Restituisce le soste in corso",
        Description = "Questo endpoint restituisce le soste in corso di tutti gli utenti.")]
    public async Task<List<Sosta>> GetStartedAdminAsync()
    {
        _logger.LogInformation("Controller soste: richiesta storico soste.");
        return await _gestoreParcheggio.RichiediSosteInCorso();
    }
}
