using System.Net;
using System.Net.Http.Headers;

namespace SmartParkWebApp.Pages;

public class PrenotaSostaModel : PageModel
{
    [BindProperty]
    public Prenotazione? Prenotazione { get; set; }
    readonly ILogger<PrenotaSostaModel> _logger;
    readonly IHttpClientFactory _httpClientFactory;
    readonly TokenJwt _tokenJwt;

    public PrenotaSostaModel(ILogger<PrenotaSostaModel> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _tokenJwt = new TokenJwt(httpContextAccessor.HttpContext);
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostPrenotazione()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("SmartPark");

            var id = _tokenJwt.EstraiIdUtente();
            if (id == Guid.Empty)
            {
                Response.Redirect("/Accedi");
            }

            // controlla campi del form per la prenotazione
            var errore = ControllaPrenotazione();
            if (!string.IsNullOrEmpty(errore))
            {
                TempData["Validazione"] = errore;
                return RedirectToPage("/PrenotaSosta");
            }

            ImpostaPrenotazione(id);

            var token = Request.Cookies["JwtToken"];
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var risposta = await httpClient.PostAsJsonAsync("/Api/Prenotazioni", Prenotazione);

            if (risposta.IsSuccessStatusCode)
            {
                _logger.LogInformation("Prenotazione inserita.");
                return RedirectToPage("/Prenotazioni");
            }
            else if (risposta.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogInformation("Prenotazione non disponibile per un utente senza abbonamento Premium.");
                return RedirectToPage("/Accedi");
            }
            else
            {
                var messaggio = "Prenotazione non disponibile per un utente senza abbonamento Premium.";
                TempData["Errore"] = messaggio;
                _logger.LogInformation($"RicaricaModel: {messaggio}");
                return RedirectToPage("/Errore");
            }
        }
        catch (Exception ex)
        {
            var messaggio = "Errore nell'inserimento della prenotazione";
            TempData["Errore"] = messaggio;
            _logger.LogInformation($"RicaricaModel: {messaggio}");
            return RedirectToPage("/Errore");
        }
    }

    private void ImpostaPrenotazione(Guid id)
    {
        Prenotazione.IdUtente = id;
        Prenotazione.Stato = false;
    }

    private string ControllaPrenotazione()
    {
        var messaggio = "";
        if (Prenotazione.DataInizio >= Prenotazione.DataFine)
        {
            messaggio = "la data di inizio deve essere antecedente a quella di fine sosta.";
        }

        return messaggio;
    }
}
