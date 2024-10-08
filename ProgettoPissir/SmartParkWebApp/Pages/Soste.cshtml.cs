using SmartParkCore.Security;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SmartParkWebApp.Pages;

public class SosteModel : PageModel
{
    [BindProperty]
    public List<Sosta>? Soste { get; set; }
    readonly ILogger<SosteModel> _logger;
    readonly IHttpClientFactory _httpClientFactory;
    readonly TokenJwt _tokenJwt;

    public SosteModel(ILogger<SosteModel> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _tokenJwt = new TokenJwt(httpContextAccessor.HttpContext);
    }

    public async Task OnGet()
    {
        Soste = null;

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
            var risposta = await httpClient.GetAsync($"/Api/Soste/InCorso/Amministratore");

            if (risposta.IsSuccessStatusCode)
            {
                var json = await risposta.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    Soste = JsonSerializer.Deserialize<List<Sosta>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            else
            {
                var messaggio = "Errore nel recupero delle soste degli utenti.";
                TempData["Errore"] = messaggio;
                _logger.LogError($"Errore prenotazione: {messaggio}");
                Response.Redirect("/Errore");
            }
        }
        else
        {
            var risposta = await httpClient.GetAsync($"/Api/Soste/InCorso/Utente?id={id}");

            if (risposta.IsSuccessStatusCode)
            {
                var json = await risposta.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    Soste = JsonSerializer.Deserialize<List<Sosta>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            else
            {
                var messaggio = $"Errore nel recupero delle soste dell'utente {id}.";
                TempData["Errore"] = messaggio;
                _logger.LogError($"Errore prenotazione: {messaggio}");
                Response.Redirect("/Errore");
            }
        }
    }
}
