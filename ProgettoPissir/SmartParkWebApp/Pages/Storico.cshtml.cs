using SmartParkCore.Security;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SmartParkWebApp.Pages;

public class StoricoModel : PageModel
{
    public IList<Ricarica>? Ricariche { get; set; }
    public IList<Sosta>? Soste { get; set; }
    public IList<Pagamento>? Pagamenti { get; set; }
    readonly ILogger<StoricoModel> _logger;
    readonly IHttpClientFactory _httpClientFactory;
    readonly TokenJwt _tokenJwt;

    public StoricoModel(ILogger<StoricoModel> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
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
            await CaricaStoricoAmministratore(httpClient);
        }
        else if (ruolo == Ruolo.Utente || ruolo == Ruolo.UtentePremium)
        {
            await CaricaStoricoUtente(httpClient, id);
        }
        else
        {
            Response.Redirect("/Accedi");
        }
    }

    private async Task CaricaStoricoUtente(HttpClient httpClient, Guid? id)
    {

        var rispostaRicariche = await httpClient.GetAsync($"/Api/Ricariche/Completate/Utente?id={id}");
        var rispostaSoste = await httpClient.GetAsync($"/Api/Soste/Utente?id={id}");
        var rispostaPagamenti = await httpClient.GetAsync($"/Api/Pagamenti/Utente?id={id}");

        if (rispostaRicariche.IsSuccessStatusCode && rispostaSoste.IsSuccessStatusCode && rispostaPagamenti.IsSuccessStatusCode)
        {
            var json = await rispostaRicariche.Content.ReadAsStringAsync();
            Ricariche = JsonSerializer.Deserialize<List<Ricarica>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            json = await rispostaSoste.Content.ReadAsStringAsync();
            Soste = JsonSerializer.Deserialize<List<Sosta>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            json = await rispostaPagamenti.Content.ReadAsStringAsync();
            Pagamenti = JsonSerializer.Deserialize<List<Pagamento>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        else if (rispostaRicariche.StatusCode == HttpStatusCode.Unauthorized && rispostaSoste.StatusCode == HttpStatusCode.Unauthorized && rispostaPagamenti.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogInformation("Accesso non autorizzato.");
            Response.Redirect("/Accedi");
        }
        else
        {
            var messaggio = "Errore nella ricerca dello storico dell'utente.";
            TempData["Errore"] = messaggio;
            Response.Redirect("/Errore");
        }
    }

    private async Task CaricaStoricoAmministratore(HttpClient httpClient)
    {
        var rispostaRicariche = await httpClient.GetAsync($"/Api/Ricariche/Completate/Amministratore");
        var rispostaSoste = await httpClient.GetAsync($"/Api/Soste/Amministratore");
        var rispostaPagamenti = await httpClient.GetAsync($"/Api/Pagamenti/Amministratore");

        if (rispostaRicariche.IsSuccessStatusCode && rispostaSoste.IsSuccessStatusCode && rispostaPagamenti.IsSuccessStatusCode)
        {
            var json = await rispostaRicariche.Content.ReadAsStringAsync();
            Ricariche = JsonSerializer.Deserialize<List<Ricarica>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            json = await rispostaSoste.Content.ReadAsStringAsync();
            Soste = JsonSerializer.Deserialize<List<Sosta>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            json = await rispostaPagamenti.Content.ReadAsStringAsync();
            Pagamenti = JsonSerializer.Deserialize<List<Pagamento>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        else if (rispostaRicariche.StatusCode == HttpStatusCode.Unauthorized && rispostaSoste.StatusCode == HttpStatusCode.Unauthorized && rispostaPagamenti.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogInformation("Accesso non autorizzato.");
            Response.Redirect("/Accedi");
        }
        else
        {
            var messaggio = "Errore nella ricerca dello storico per l'amministratore.";
            TempData["Errore"] = messaggio;
            Response.Redirect("/Errore");
        }
    }
}
