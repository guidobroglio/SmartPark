using System.Net.Http.Headers;

namespace SmartParkWebApp.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public Sosta? Sosta { get; set; }
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<IndexModel> _logger;    

    public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;        
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostSosta()
    {
        var httpClient = _httpClientFactory.CreateClient("SmartPark");

        var token = Request.Cookies["JwtToken"];
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var risposta = await httpClient.PostAsync("/Api/Soste/Richiesta", null);
        if (risposta.IsSuccessStatusCode)
        {
            var posto = await risposta.Content.ReadFromJsonAsync<int>();
            if (posto == -1)
            {
                var messaggio = "Non ci sono posti disponibili.";
                _logger.LogError($"Errore richiesta sosta: {messaggio}");

                TempData["Errore"] = messaggio;
                return RedirectToPage("/Errore");
            }
            else
            {
                _logger.LogInformation($"Conferma richiesta sosta al posto {posto}");
                return RedirectToPage("/Sosta", new { posto });
            }
        }
        else
        {
            var messaggio = "Errore nel calcolo dei posti disponibili.";
            TempData["Errore"] = messaggio;
            _logger.LogInformation($"IndexModel: {messaggio}");
            return RedirectToPage("/Errore");
        }
    }
}
