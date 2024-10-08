using SmartParkCore.Security;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SmartParkWebApp.Pages;

public class PrenotazioniModel : PageModel
{
    [BindProperty]
    public List<Prenotazione>? Prenotazioni { get; set; }
    readonly ILogger<PrenotazioniModel> _logger;
    readonly IHttpClientFactory _httpClientFactory;
    readonly TokenJwt _tokenJwt;

    public PrenotazioniModel(ILogger<PrenotazioniModel> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _tokenJwt = new TokenJwt(httpContextAccessor.HttpContext);
    }

    public async Task OnGet()
    {
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
            var risposta = await httpClient.GetAsync($"/Api/Prenotazioni/InSospeso/Amministratore");

            if (risposta.IsSuccessStatusCode)
            {
                var json = await risposta.Content.ReadAsStringAsync();
                Prenotazioni = JsonSerializer.Deserialize<List<Prenotazione>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                var messaggio = "Errore nel recupero delle prenotazioni degli utenti.";
                TempData["Errore"] = messaggio;
                _logger.LogError($"Errore prenotazione: {messaggio}");
                Response.Redirect("/Errore");
            }
        }
        else
        {
            var risposta = await httpClient.GetAsync($"/Api/Prenotazioni/InSospeso/Utente?id={id}");

            if (risposta.IsSuccessStatusCode)
            {
                var json = await risposta.Content.ReadAsStringAsync();
                Prenotazioni = JsonSerializer.Deserialize<List<Prenotazione>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                var messaggio = $"Errore nel recupero delle prenotazioni dell'utente {id}.";
                TempData["Errore"] = messaggio;
                _logger.LogError($"Errore prenotazione: {messaggio}");
                Response.Redirect("/Errore");
            }
        }
    }

}

