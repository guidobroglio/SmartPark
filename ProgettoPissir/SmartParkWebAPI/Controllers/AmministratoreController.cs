using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartParkCore.Models.Json;
using SmartParkCore.Security;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartParkWebAPI.Controllers;

[ApiController]
[Route("/Api/[controller]")]
public class AmministratoreController : Controller
{
    readonly ILogger<AmministratoreController> _logger;
    readonly IGestorePagamenti _gestorePagamenti;
    readonly IGestorePrezzi _gestorePrezzi;
    readonly IGestoreParcheggio _gestoreParcheggio;

    public AmministratoreController(ILogger<AmministratoreController> logger, IGestorePagamenti gestorePagamenti, 
        IGestorePrezzi gestorePrezzi, IGestoreParcheggio gestoreParcheggio)
    {
        _logger = logger;
        _gestorePagamenti = gestorePagamenti;
        _gestorePrezzi = gestorePrezzi;
        _gestoreParcheggio = gestoreParcheggio;
    }

    // GET Api/Amministratore
    [Authorize(Roles = Ruolo.Amministratore)]
    [HttpGet]
    [SwaggerOperation(Summary = "Restituisce informazioni utili per l'amministratore",
        Description = "Questo endpoint trova l'occupazione dei posti e i valori attuali dei prezzi di sosta e di ricarica.")]
    public async Task<InfoJson> GetAsync()
    {
        _logger.LogInformation("Controller amministratore: restituiti posti, prezzi e pagamenti.");
        var occupazioni = await _gestoreParcheggio.RichiediOccupazionePosti();
        var prezzo = await _gestorePrezzi.RichiediPrezzi();

        var info = new InfoJson
        {
            Occupazioni = occupazioni,
            Prezzo = prezzo
        };
        
        return info;
    }

    // PUT Api/Amministratore/Prezzi
    [Authorize(Roles = Ruolo.Amministratore)]
    [HttpPut("Prezzi")]
    [SwaggerOperation(Summary = "Aggiorna i prezzi di sosta e ricarica",
        Description = "Questo endpoint permette di aggiornare i prezzi di sosta e ricarica.")]
    public async Task PostAsync(Prezzo prezzo)
    {
        _logger.LogInformation("Controller amministratore: aggiornati i prezzi di sosta e ricarica.");
        await _gestorePrezzi.ModificaPrezzi(prezzo);
    }
}
