using System.Net.Http.Headers;

namespace SmartParkWebApp.Pages;

public class SostaModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int IdPosto { get; set; }

    [BindProperty]
    public Sosta? Sosta { get; set; }

    readonly ILogger<SostaModel> _logger;
    readonly IHttpClientFactory _httpClientFactory;
    readonly TokenJwt _tokenJwt;

    public SostaModel(ILogger<SostaModel> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _tokenJwt = new TokenJwt(httpContextAccessor.HttpContext);
    }

    public void OnGet(int posto)
    {
        IdPosto = posto;
        _logger.LogInformation($"SostaModel: posto ricevuto {posto}");
    }

    public async Task<IActionResult> OnPost()
    {
        var httpClient = _httpClientFactory.CreateClient("SmartPark");

        var id = _tokenJwt.EstraiIdUtente();

        if (id == Guid.Empty)
        {
            return RedirectToPage("/Accedi");
        }

        ImpostaSosta(id);

        _logger.LogInformation($"SostaModel: Inserimento sosta al posto: {Sosta.Posto.Id} per l'utente {id.ToString()}");

        var token = Request.Cookies["JwtToken"];
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var risposta = await httpClient.PostAsJsonAsync("/Api/Soste/Inserimento", Sosta);

        if (risposta.IsSuccessStatusCode)
        {
            var esito = await risposta.Content.ReadFromJsonAsync<int>();
            var messaggio = "";
            switch (esito)
            {
                case 0:
                    _logger.LogInformation("SostaModel: inserimento sosta completato");
                    return RedirectToPage("/Soste"); 
                case 1:
                    messaggio = "Veicolo non riconosciuto.";
                    _logger.LogInformation($"SostaModel: {messaggio}");
                    TempData["Errore"] = messaggio;
                    return RedirectToPage("/Errore");
                case 2:
                    messaggio = "Veicolo già in sosta.";
                    _logger.LogInformation($"SostaModel: {messaggio}");
                    TempData["Errore"] = messaggio;
                    return RedirectToPage("/Errore");
                default:
                    _logger.LogInformation($"SostaModel: {messaggio}");
                    TempData["Errore"] = messaggio;
                    return RedirectToPage("/Errore");
            }
        }
        else
        {
            var messaggio = "Errore nell'inserimento della sosta.";
            _logger.LogInformation($"SostaModel: {messaggio}");
            TempData["Errore"] = messaggio;
            return RedirectToPage("/Errore");
        }
    }

    private void ImpostaSosta(Guid id)
    {
        Sosta.DataInizio = DateTime.Now;
        Sosta.DataFine = DateTime.Now;
        Sosta.Posto = new Posto
        {
            Id = IdPosto,
            Disponibile = true
        };
        Sosta.Veicolo.IdUtente = id; 
        Sosta.IdUtente = id;
    }
}
