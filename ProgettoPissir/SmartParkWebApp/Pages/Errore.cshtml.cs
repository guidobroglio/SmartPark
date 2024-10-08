namespace SmartParkWebApp.Pages;

public class ErroreModel : PageModel
{
    private readonly ILogger<ErroreModel> _logger;

    public string Messaggio;

    public ErroreModel(ILogger<ErroreModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        Messaggio = (string) TempData["Errore"];        
    }
}
