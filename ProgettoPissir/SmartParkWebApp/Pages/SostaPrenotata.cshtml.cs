using System.Net.Http.Headers;

namespace SmartParkWebApp.Pages;

public class SostaPrenotataModel : PageModel
{
    [BindProperty]
    public Sosta? Sosta { get; set; }

    readonly ILogger<SostaPrenotataModel> _logger;
    readonly IHttpClientFactory _httpClientFactory;
    readonly TokenJwt _tokenJwt;

    public SostaPrenotataModel(ILogger<SostaPrenotataModel> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _tokenJwt = new TokenJwt(httpContextAccessor.HttpContext);
    }
    public void OnGet()
    {
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

        var token = Request.Cookies["JwtToken"];
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var risposta = await httpClient.PostAsJsonAsync("/Api/Prenotazioni/Sosta", Sosta);

        if (risposta.IsSuccessStatusCode)
        {
            var esito = await risposta.Content.ReadFromJsonAsync<int>();
            var messaggio = "";
            switch (esito)
            {
                case 0:
                    _logger.LogInformation("SostaPrenotataModel: inserimento sosta completato");
                    return RedirectToPage("/Soste");
                case 1:
                    messaggio = "Veicolo non riconosciuto.";
                    _logger.LogInformation($"SostaPrenotataModel: {messaggio}");
                    TempData["Errore"] = messaggio;
                    return RedirectToPage("/Errore");
                case 2:
                    messaggio = "Veicolo già in sosta.";
                    _logger.LogInformation($"SostaModel: {messaggio}");
                    TempData["Errore"] = messaggio;
                    return RedirectToPage("/Errore");
                case 3:
                    messaggio = "Sei arrivato oltre l'orario previsto, è stata addebitata una penale di 5.00 €.";
                    _logger.LogInformation($"SostaPrenotataModel: {messaggio}");
                    TempData["Errore"] = messaggio;
                    return RedirectToPage("/Errore");
                case 4:
                    messaggio = "Sei arrivato in anticipo rispetto all'orario previsto.";
                    _logger.LogInformation($"SostaPrenotataModel: {messaggio}");
                    TempData["Errore"] = messaggio;
                    return RedirectToPage("/Errore");
                case 5:
                    messaggio = "Nessuna prenotazione trovata.";
                    _logger.LogInformation($"SostaModel: {messaggio}");
                    TempData["Errore"] = messaggio;
                    return RedirectToPage("/Errore");
                default:
                    messaggio = "Errore nell'inserimento della sosta.";
                    _logger.LogInformation($"SostaPrenotataModel: {messaggio}");
                    TempData["Errore"] = messaggio;
                    return RedirectToPage("/Errore");
            }
        }
        else
        {
            var messaggio = "Errore nell'inserimento della sosta.";
            TempData["Errore"] = messaggio;
            _logger.LogError($"SostaPrenotataModel: {messaggio}");
            return RedirectToPage("/Errore");
        }
    }

    private void ImpostaSosta(Guid id)
    {
        Sosta.DataInizio = DateTime.Now;
        Sosta.DataFine = DateTime.Now;
        Sosta.Posto = new Posto
        {
            Id = 0,
            Disponibile = true
        };
        Sosta.Veicolo.IdUtente = id;
        Sosta.IdUtente = id;
    }
}
