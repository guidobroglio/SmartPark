using SmartParkCore.Security;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;

namespace SmartParkWebApp.Pages;

public class AreaPersonaleModel : PageModel
{
    [BindProperty]
    public Registrazione? Utente { get; set; }
    readonly ILogger<AreaPersonaleModel> _logger;
    readonly IHttpClientFactory _httpClientFactory;
    readonly TokenJwt _tokenJwt;

    public AreaPersonaleModel(ILogger<AreaPersonaleModel> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _tokenJwt = new TokenJwt(httpContextAccessor.HttpContext);
    }

    public async Task OnGet()
    {
        Utente = null;
 
        var httpClient = _httpClientFactory.CreateClient("SmartPark");

        var id = _tokenJwt.EstraiIdUtente();

        if (id == Guid.Empty)
        {
            _logger.LogInformation("Accesso non autorizzato.");
            Response.Redirect("/Accedi");
        }

        var token = Request.Cookies["JwtToken"];
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var risposta = await httpClient.GetAsync($"/Api/Accesso?id={id}");

        if (risposta.IsSuccessStatusCode)
        {
            var json = await risposta.Content.ReadAsStringAsync();

            Utente = JsonSerializer.Deserialize<Registrazione>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        }
        else if (risposta.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogInformation("Accesso non autorizzato.");
            Response.Redirect("/Accedi");
        }
        else
        {
            var messaggio = "Errore nell'accesso ai dati utente.";
            TempData["Errore"] = messaggio;
            _logger.LogError($"Errore area personale: {messaggio}");
            Response.Redirect("/Errore");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("SmartPark");

        // controlla campi del form per le credenziali
        var errore = ControllaCredenziali();
        if (!string.IsNullOrEmpty(errore))
        {
            TempData["Validazione"] = errore;
            return RedirectToPage("/AreaPersonale");
        }

        var risposta = await httpClient.PutAsJsonAsync("/Api/Accesso/Aggiorna", Utente);

        if (risposta.IsSuccessStatusCode)
        {
            // ottiene il token jwt
            var token = await risposta.Content.ReadFromJsonAsync<TokenJson>();
            _logger.LogInformation($"Generated JWT Token: {token.Token}");
            if (token != null)
            {
                Response.Cookies.Append("JwtToken", token.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTimeOffset.Now.AddHours(1)
                });

                return RedirectToPage("/Index");
            }
            else
            {
                TempData["Errore"] = "Registrazione fallita";
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
            _logger.LogInformation($"Errore nell'aggiornamento delle credenziali: {risposta.Content.ReadAsStringAsync()}");
            TempData["Errore"] = "Errore nell'aggiornamento delle credenziali.";
            return RedirectToPage("/Errore");
        }
    }

    private string ControllaCredenziali()
    {
        var messaggio = "";

        foreach (var veicolo in Utente.Veicoli)
        {
            if (string.IsNullOrEmpty(veicolo.Targa))
            {
                messaggio = "inserire la targa del veicolo.";
            }

            if (veicolo.CapacitaBatteria <= 0)
            {
                messaggio = "inserire la capacità della batteria.";
            }
        }
        if (string.IsNullOrEmpty(Utente.NumeroCarta))
        {
            messaggio = "inserire il numero della carta.";
        }

        if (string.IsNullOrEmpty(Utente.Nome))
        {
            messaggio = "inserire il nome.";
        }

        if (string.IsNullOrEmpty(Utente.Cognome))
        {
            messaggio = "inserire il cognome.";
        }

        if (Utente.Password.Length < 8)
        {
            messaggio = "la password deve avere almeno 8 caratteri.";
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(Utente.Password, @"[!@#$%^&*(),.?""{}|<>]"))
        {
            messaggio = "la password deve contenere almeno un carattere speciale.";
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(Utente.Password, @"\d"))
        {
            messaggio = "la password deve contenere almeno un numero.";
        }

        if (!Utente.Email.Contains("@") || !Utente.Email.Contains("."))
        {
            messaggio = "inserire un'email valida.";
        }

        return messaggio;
    }
}
