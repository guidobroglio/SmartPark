using SmartParkCore.Security;

namespace SmartParkWebApp.Pages;

public class AccediModel : PageModel
{
    [BindProperty]
    public Accesso? Utente { get; set; }
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AccediModel> _logger;

    public AccediModel(IHttpClientFactory httpClientFactory, ILogger<AccediModel> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostLoginAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("SmartPark");

        // controlla campi del form per le credenziali
        var errore = ControllaCredenziali();
        if (!string.IsNullOrEmpty(errore))
        {
            TempData["Validazione"] = errore;
            return RedirectToPage("/RegistratiPremium");
        }

        var risposta = await httpClient.PostAsJsonAsync("/Api/Accesso/Accedi", Utente);

        if (risposta.IsSuccessStatusCode)
        {
            var token = await risposta.Content.ReadFromJsonAsync<TokenJson>();

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
                var messaggio = "Credenziali errate.";
                TempData["Errore"] = messaggio;
                _logger.LogInformation($"AccediModel: {messaggio}");
                return RedirectToPage("/Errore");
            }
        }
        else
        {
            var messaggio = "Errore nell'accesso.";
            TempData["Errore"] = messaggio;
            _logger.LogInformation($"AccediModel: {messaggio}");
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

    public IActionResult OnPostLogout()
    { 
        Response.Cookies.Delete("JwtToken");
        return RedirectToPage("/Index");
    }
}
