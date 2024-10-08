using System.Net;
using SmartParkCore.Security;

namespace SmartParkWebApp.Pages;

public class RegistratiPremiumModel : PageModel
{
    [BindProperty]
    public Accesso? Utente { get; set; }
    readonly ILogger<RegistratiPremiumModel> _logger;
    readonly IHttpClientFactory _httpClientFactory;

    public RegistratiPremiumModel(ILogger<RegistratiPremiumModel> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
        var httpClient = _httpClientFactory.CreateClient("SmartPark");

        // controlla campi del form per le credenziali
        var errore = ControllaCredenziali();
        if (!string.IsNullOrEmpty(errore))
        {
            TempData["Validazione"] = errore;
            return RedirectToPage("/RegistratiPremium");
        }

        var risposta = await httpClient.PostAsJsonAsync("/Api/Accesso/RegistratiPremium", Utente);

        if (risposta.IsSuccessStatusCode)
        {
            var nuovoToken = await risposta.Content.ReadFromJsonAsync<TokenJson>();
            // cancella vecchio token
            Response.Cookies.Delete("JwtToken");

            if (nuovoToken != null)
            {
                Response.Cookies.Append("JwtToken", nuovoToken.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTimeOffset.Now.AddHours(1)
                });

                return RedirectToPage("/Index");
            }
            else
            {
                TempData["Errore"] = risposta.Content.ToString();
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
            TempData["Errore"] = risposta.Content.Headers.ToString();
            return RedirectToPage("/Errore");
        }
    }

    private string ControllaCredenziali()
    {
        var messaggio = "";
        
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
