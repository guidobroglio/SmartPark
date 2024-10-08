using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartParkPhilipsHue;

// Programma principale
public class Program
{
    // Metodo principale
    public static async Task Main(string[] args)
    {
        // Configurazione del provider di servizi per la dependency injection
        var serviceProvider = new ServiceCollection()
            // Aggiungi il servizio di logging
            .AddLogging(configure =>
            {
                // Configura il logging su console
                configure.AddConsole();
                configure.SetMinimumLevel(LogLevel.Information); // Imposta il livello minimo di log
            })
            // Aggiungi il servizio di gestione delle luci Philips Hue
            .BuildServiceProvider();

        // Ottieni il logger
        var logger = serviceProvider.GetService<ILogger<GestoreLuciHue>>();

        // Crea l'istanza di GestoreLuciHue con il logger
        var gestore = new GestoreLuciHue("localhost:8000", "newdeveloper", logger);

        // Prova accensione, invia un comando per impostare lampadina 1 gialla, 2 rossa, 3 verde
        await gestore.InviaComandoAsync("1", "{\"on\":true, \"hue\":12750, \"sat\":254, \"bri\":254}");
        // Richiama lo stato della lampadina 1
        await gestore.RichiamaStatoLuce("1");
        // Attendi 1 secondo
        await gestore.ImpostaColoreRossoAsync("2");
        // Richiama lo stato della lampadina 2
        await gestore.RichiamaStatoLuce("2");
        // Attendi 1 secondo
        await gestore.ImpostaColoreVerdeAsync("3");
        // Richiama lo stato della lampadina 3
        await gestore.RichiamaStatoLuce("3");
    }
}
