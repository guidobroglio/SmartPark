using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartParkCore.Security;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartParkWebAPI.Controllers;

[ApiController]
[Route("/Api/[controller]")]
public class PrenotazioniController : Controller
{
    readonly ILogger<PrenotazioniController> _logger;
    readonly IGestorePrenotazioni _gestorePrenotazioni;
    readonly IGestoreParcheggio _gestoreParcheggio;

    public PrenotazioniController(ILogger<PrenotazioniController> logger, IGestorePrenotazioni gestorePrenotazioni, IGestoreParcheggio gestoreParcheggio)
    {
        _logger = logger;
        _gestorePrenotazioni = gestorePrenotazioni;
        _gestoreParcheggio = gestoreParcheggio;
    }

    // POST Api/Prenotazioni
    [Authorize(Roles = Ruolo.UtentePremium)]
    [HttpPost]
    [SwaggerOperation(Summary = "Inserisce una prenotazione",
        Description = "Questo endpoint permette di inserire una prenotazione per una sosta.")]
    public async Task PostPrenotazioneAsync(Prenotazione prenotazione)
    {
        _logger.LogInformation("Controller prenotazione: inserimento prenotazione");
        await _gestorePrenotazioni.InserisciPrenotazione(prenotazione);
    }

    // POST Api/Prenotazioni/Sosta
    [Authorize(Roles = Ruolo.UtentePremium)]
    [HttpPost("Sosta")]
    [SwaggerOperation(Summary = "Inserisce una sosta prenotata",
        Description = "Questo endpoint permette di inserire una sosta usando una prenotazione.")]
    public async Task<int> PostSostaAsync(Sosta sosta)
    {
        _logger.LogInformation("Controller prenotazioni: inserimento sosta prenotata.");
        return await _gestoreParcheggio.InserisciSostaPrenotata(sosta);
    }

    // GET Api/Prenotazioni/InSospeso/Utente
    [Authorize(Roles = Ruolo.Utente + "," + Ruolo.UtentePremium)]
    [HttpGet("InSospeso/Utente")]
    [SwaggerOperation(Summary = "Ritorna le prenotazioni in sospeso",
        Description = "Questo endpoint restituisce le prenotazioni in sospeso per l'utente.")]
    public async Task<List<Prenotazione>> GetInSospesoIdAsync(Guid id)
    {
        _logger.LogInformation("Controller prenotazioni: richiesta prenotazioni in sospeso.");
        return await _gestorePrenotazioni.RichiediPrenotazioniInSospeso(id);
    }

    // GET Api/Prenotazioni/InSospeso/Amministratore
    [Authorize(Roles = Ruolo.Amministratore)]
    [HttpGet("InSospeso/Amministratore")]
    [SwaggerOperation(Summary = "Ritorna le prenotazioni in sospeso",
        Description = "Questo endpoint restituisce le prenotazioni in sospeso per tutti gli utenti.")]
    public async Task<List<Prenotazione>> GetInSospesoIdAsync()
    {
        _logger.LogInformation("Controller prenotazioni: richiesta prenotazioni in sospeso.");
        return await _gestorePrenotazioni.RichiediPrenotazioniInSospeso();
    }
}
