using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartParkCore.Models.Json;
using SmartParkCore.Security;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartParkWebAPI.Controllers;

[ApiController]
[Route("/Api/[controller]")]
public class PagamentiController : ControllerBase
{
    readonly ILogger<PagamentiController> _logger;
    readonly IGestorePagamenti _gestorePagamenti;

    public PagamentiController(ILogger<PagamentiController> logger, IGestorePagamenti gestorePagamenti)
    {
        _logger = logger;
        _gestorePagamenti = gestorePagamenti;
    }

    // POST Api/Pagamenti
    [Authorize(Roles = Ruolo.Utente + "," + Ruolo.UtentePremium)]
    [HttpPost]
    [SwaggerOperation(Summary = "Inserisce un pagamento",
        Description = "Questo endpoint permette di inserire un nuovo pagamento per una sosta ed eventuali ricariche.")]
    public async Task<int> PostAsync(UtenteTargaJson targa)
    {
        _logger.LogInformation("Controller pagamento: inserimento pagamento");
        return await _gestorePagamenti.InserisciPagamento(targa.Targa);
    }

    // GET Api/Pagamenti/Utente
    [Authorize(Roles = Ruolo.Utente + "," + Ruolo.UtentePremium)]
    [HttpGet("Utente")]
    [SwaggerOperation(Summary = "Ritorna lo storico dei pagamenti dell'utente",
        Description = "Questo endpoint restituisce la lista dei pagamenti dell'utente.")]
    public async Task<List<Pagamento>> GetUserAsync(Guid id)
    {
        _logger.LogInformation("Controller ricarica: richiesta storico pagamenti.");
        return await _gestorePagamenti.RichiediPagamentiUtente(id);
    }

    // GET Api/Pagamenti/Amministratore
    [Authorize(Roles = Ruolo.Amministratore)]
    [HttpGet("Amministratore")]
    [SwaggerOperation(Summary = "Ritorna lo storico dei pagamenti di tutti gli utenti",
        Description = "Questo endpoint restituisce la lista dei pagamenti di tutti gli utenti.")]
    public async Task<List<Pagamento>> GetAdminAsync()
    {
        _logger.LogInformation("Controller ricarica: richiesta storico pagamenti.");
        return await _gestorePagamenti.RichiediPagamenti();
    }
}
