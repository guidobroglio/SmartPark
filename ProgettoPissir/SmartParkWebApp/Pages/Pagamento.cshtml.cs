using SmartParkCore.Models.Json;
using System.Net;
using System.Net.Http.Headers;

namespace SmartParkWebApp.Pages;

public class PagamentoModel : PageModel
{
    [BindProperty]
    public UtenteTargaJson? UtenteTarga { get; set; }
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PagamentoModel> _logger;
    readonly TokenJwt _tokenJwt;    

    public PagamentoModel(ILogger<PagamentoModel> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
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
        try
        {
            var httpClient = _httpClientFactory.CreateClient("SmartPark");

            // controlla campi del form per il pagamento
            var errore = ControllaPagamento();
            if (!string.IsNullOrEmpty(errore))
            {
                TempData["Validazione"] = errore;
                return RedirectToPage("/Pagamento");
            }

            var id = _tokenJwt.EstraiIdUtente();
            UtenteTarga.IdUtente = id;
            _logger.LogInformation($"Pagamento: pagamento per il veicolo {UtenteTarga.Targa} dell'utente {UtenteTarga.IdUtente}");

            var token = Request.Cookies["JwtToken"];
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var risposta = await httpClient.PostAsJsonAsync("/Api/Pagamenti", UtenteTarga);

            if (risposta.IsSuccessStatusCode)
            {
                var esito = await risposta.Content.ReadFromJsonAsync<int>();
                if (esito == 0)
                {
                    _logger.LogInformation("Pagamento effettuato.");
                    return RedirectToPage("/Storico");
                }
                else
                {
                    var messaggio = "Nessuna sosta attiva, impossibile effettuare il pagamento.";
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
                var messaggio = "Errore nel pagamento";
                TempData["Errore"] = messaggio;
                _logger.LogInformation($"RicaricaModel: {messaggio}");
                return RedirectToPage("/Errore");
            }
        }
        catch (Exception ex)
        {
            var messaggio = "Errore nel pagamento";
            TempData["Errore"] = messaggio;
            _logger.LogError($"Errore pagamento: {ex.Message}");
            return RedirectToPage("/Errore");
        }
    }

    private string ControllaPagamento()
    {
        var messaggio = "";
        if (string.IsNullOrEmpty(UtenteTarga.Targa))
        {
            messaggio = "inserire la targa del veicolo.";
        }

        return messaggio;
    }
}
