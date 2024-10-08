using SmartParkCore.Models.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SmartParkWebApp.Pages;

public class PannelloControlloModel : PageModel
{
    [BindProperty]
    public IList<Posto>? Posti { get; set; }
    [BindProperty]
    public IList<Pagamento>? Pagamenti { get; set; }
    [BindProperty]
    public Prezzo? Prezzo { get; set; }
    [BindProperty]
    public InfoJson? Info { get; set; }
    readonly ILogger<PannelloControlloModel> _logger;
    readonly IHttpClientFactory _httpClientFactory;


    public PannelloControlloModel(ILogger<PannelloControlloModel> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task OnGet()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("SmartPark");

            var token = Request.Cookies["JwtToken"];
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var risposta = await httpClient.GetAsync("/Api/Amministratore");

            if (risposta.IsSuccessStatusCode)
            {
                var json = await risposta.Content.ReadAsStringAsync();
                
                Info = JsonSerializer.Deserialize<InfoJson>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            }
            else if (risposta.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogInformation("Accesso non autorizzato.");
                Response.Redirect("/Accedi");
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Errore storico utenti: {e.Message}");
        }
    }

    public async Task<IActionResult> OnPost()
    {
        var httpClient = _httpClientFactory.CreateClient("SmartPark");

        var token = Request.Cookies["JwtToken"];
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var risposta = await httpClient.PutAsJsonAsync("/Api/Amministratore/Prezzi", Prezzo);

        if (risposta.IsSuccessStatusCode)
        {             
            return RedirectToPage("/PannelloControllo");
        }
        else
        {
            TempData["Errore"] = "Impossibile aggiornare i prezzi.";
            return RedirectToPage("/Errore");
        }
    }
}
