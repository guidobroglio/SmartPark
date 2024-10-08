using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartParkCore.Security;

namespace SmartParkWebAPI.Controllers;

[ApiController]
[Route("/Api/[controller]")]
public class VeicoliController : Controller
{
    readonly ILogger<VeicoliController> _logger;
    readonly IRepositoryVeicolo _repositoryVeicolo;

    public VeicoliController(ILogger<VeicoliController> logger, IRepositoryVeicolo repositoryVeicolo)
    {
        _logger = logger;
        _repositoryVeicolo = repositoryVeicolo;
    }

    // GET Api/Veicoli
    [Authorize(Roles = Ruolo.Utente + "," + Ruolo.UtentePremium)]
    [HttpGet]
    public async Task<List<Veicolo>> GetAsync()
    {
        _logger.LogInformation("Controller veicoli: richiesta veicoli");
        return await _repositoryVeicolo.RichiediVeicoli();
    }
}
