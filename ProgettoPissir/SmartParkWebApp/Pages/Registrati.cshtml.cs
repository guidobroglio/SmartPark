using SmartParkCore.Security;

namespace SmartParkWebApp.Pages;

public class RegistrazioneModel : PageModel
{
    [BindProperty]
    public Registrazione? Utente { get; set; }
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<RegistrazioneModel> _logger;

    public RegistrazioneModel(IHttpClientFactory httpClientFactory, ILogger<RegistrazioneModel> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }
    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostRegisterAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("SmartPark");

        // controlla campi del form per le credenziali
        var errore = ControllaCredenziali();
        if (!string.IsNullOrEmpty(errore))
        {
            TempData["Validazione"] = errore;
            return RedirectToPage("/Registrati");
        }

        var risposta = await httpClient.PostAsJsonAsync("/Api/Accesso/Registrati", Utente);

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
        else
        {
            TempData["Errore"] = "Richiesta API fallita";
            return RedirectToPage("/Errore");
        }
    }

    private string ControllaCredenziali()
    {
        var messaggio = "";

        foreach(var veicolo in Utente.Veicoli)
        {
            if (string.IsNullOrEmpty(veicolo.Targa))
            {
                messaggio = "inserire la targa del veicolo.";
            }

            if (veicolo.CapacitaBatteria<=0)
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
