using System.Text.Json;
using System.Net.Http.Headers;
using System.Net;
using SmartParkCore.Security;

namespace SmartParkWebApp.Pages;

public class RicaricaModel : PageModel
{
    [BindProperty]
    public Ricarica? Ricarica { get; set; }

    [BindProperty]
    public Ricarica? RicaricaAttiva { get; set; }

    [BindProperty]
    public Veicolo? Veicolo { get; set; }

    public int? RicaricheInCoda { get; set; }

    readonly ILogger<RicaricaModel> _logger;
    readonly IHttpClientFactory _httpClientFactory;
    readonly TokenJwt _tokenJwt;

    public RicaricaModel(ILogger<RicaricaModel> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _tokenJwt = new TokenJwt(httpContextAccessor.HttpContext);
    }

    public async Task OnGet()
    {
        RicaricaAttiva = null;
        RicaricheInCoda = 0;

        var httpClient = _httpClientFactory.CreateClient("SmartPark");

        var id = _tokenJwt.EstraiIdUtente();

        if (id == Guid.Empty)
        {
            Response.Redirect("/Accedi");
        }

        var ruolo = _tokenJwt.EstraiRuolo();

        var token = Request.Cookies["JwtToken"];
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (ruolo == Ruolo.Amministratore)
        {
            await CaricaAmministratore(httpClient);
        }
        else if (ruolo == Ruolo.Utente || ruolo == Ruolo.UtentePremium)
        {
            await CaricaRicaricheInCorso(httpClient, id);
        }
        else
        {
            Response.Redirect("/Accedi");
        }
    }

    private async Task CaricaAmministratore(HttpClient httpClient)
    {
        var risposta = await httpClient.GetAsync("Api/Ricariche/MWbot");
        if (risposta.IsSuccessStatusCode)
        {
            if (risposta.Content != null)
            {
                var json = await risposta.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    RicaricaAttiva = JsonSerializer.Deserialize<Ricarica>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    _logger.LogInformation($"Ricarica in corso: ricarica al veicolo {RicaricaAttiva.Veicolo.Targa} in corso.");
                }
            }
        }
        else
        {
            _logger.LogInformation("Errore nel recupero della ricarica in corso.");
            TempData["Errore"] = "Errore nel recupero della ricarica in corso.";
            Response.Redirect("/Errore");
        }
    }

    private async Task CaricaRicaricheInCorso(HttpClient httpClient, Guid id)
    {
        var risposta = await httpClient.GetAsync($"/Api/Ricariche/InCorso?id={id}");
        if (risposta.IsSuccessStatusCode)
        {
            if (risposta.Content != null)
            {
                var json = await risposta.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    RicaricaAttiva = JsonSerializer.Deserialize<Ricarica>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }

            if (RicaricaAttiva != null)
            {
                _logger.LogInformation($"Ricarica attivata in data {RicaricaAttiva.DataInizio}.");
            }
            else
            {
                _logger.LogInformation("Nessuna ricarica in corso.");
                await CaricaRicaricheInCoda(httpClient);
            }
        }
        else if (risposta.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogInformation("Accesso non autorizzato.");
            Response.Redirect("/Accedi");
        }
        else
        {
            _logger.LogInformation($"Errore: {risposta.StatusCode}");
            TempData["Errore"] = "Errore nel recupero delle ricariche.";
            Response.Redirect("/Errore");
        }
    }

    private async Task CaricaRicaricheInCoda(HttpClient httpClient)
    {
        var risposta = await httpClient.GetAsync("Api/Ricariche/InCoda");
        if (risposta.IsSuccessStatusCode)
        {
            var json = await risposta.Content.ReadAsStringAsync();
            RicaricheInCoda = JsonSerializer.Deserialize<int>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            _logger.LogInformation($"Ricariche in coda: {RicaricheInCoda}");
        }
        else
        {
            _logger.LogInformation("Errore nel recupero delle ricariche in coda.");
            TempData["Errore"] = "Errore nel recupero delle ricariche in coda.";
            Response.Redirect("/Errore");
        }
    }

    public async Task<IActionResult> OnPost()
    {
        var httpClient = _httpClientFactory.CreateClient("SmartPark");

        var id = _tokenJwt.EstraiIdUtente();

        if (id == Guid.Empty)
        {
            return RedirectToPage("/Accedi");
        }

        // controlla campi del form per la ricarica
        var errore = ControllaRicarica();
        if (!string.IsNullOrEmpty(errore))
        {
            TempData["Validazione"] = errore;
            return RedirectToPage("/Ricarica");
        }

        // imposta i campi rimanenti
        ImpostaRicarica(id);

        var token = Request.Cookies["JwtToken"];
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        _logger.LogInformation($"RicaricaModel: richiesta ricarica al posto {Ricarica.IdPosto}");

        var risposta = await httpClient.PostAsJsonAsync("/Api/Ricariche", Ricarica);

        if (risposta.IsSuccessStatusCode)
        {
            var esito = await risposta.Content.ReadFromJsonAsync<int>();
            switch (esito)
            {
                case 0:
                    _logger.LogInformation("RicaricaModel: richiesta ricarica completata");
                    return RedirectToPage("/Index");
                case 1:
                    var messaggio = "Richiesta di ricarica in attesa, MWbot occupato.";
                    TempData["Errore"] = messaggio;
                    _logger.LogInformation($"RicaricaModel: {messaggio}");
                    return RedirectToPage("/Errore");
                case 2:
                    messaggio = "Ricarica non disponibile, veicolo non in sosta al momento.";
                    TempData["Errore"] = messaggio;
                    _logger.LogInformation($"RicaricaModel: {messaggio}");
                    return RedirectToPage("/Errore");
                default:
                    messaggio = "Errore nella ricarica";
                    TempData["Errore"] = messaggio;
                    _logger.LogInformation($"RicaricaModel: {messaggio}");
                    return RedirectToPage("/Errore");
            }
        }
        else if (risposta.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogInformation("Accesso non autorizzato.");
            return RedirectToPage("/Accedi");
        }
        else
        {
            var messaggio = "Errore nella ricarica";
            TempData["Errore"] = messaggio;
            _logger.LogInformation($"RicaricaModel: {messaggio}");
            return RedirectToPage("/Errore");
        }

    }

    private void ImpostaRicarica(Guid id)
    {
        Ricarica.IdUtente = id;
        Ricarica.DataInizio = DateTime.Now;
        Ricarica.DataFine = DateTime.Now;
        Ricarica.Veicolo = new Veicolo
        {
            PercentualeBatteria = Veicolo.PercentualeBatteria,
            IdUtente = id
        };
    }

    private string ControllaRicarica()
    {
        var messaggio = "";
        if (Veicolo.PercentualeBatteria < 0 || Veicolo.PercentualeBatteria > 100 ||
            Ricarica.PercentualeRicarica < 0 || Ricarica.PercentualeRicarica > 100)
        {
            messaggio = "la percentuale della batteria deve essere compresa tra 0 e 100.";
        }
        if (Veicolo.PercentualeBatteria >= Ricarica.PercentualeRicarica)
        {
            messaggio = "la percentuale attuale deve essere minore di quella richiesta.";
        }

        return messaggio;
    }

}
