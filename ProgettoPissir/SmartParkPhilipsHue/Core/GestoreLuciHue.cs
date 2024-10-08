using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Logging;

// GestoreLuciHue è un servizio che si occupa della gestione delle luci Philips Hue
namespace SmartParkPhilipsHue
{

    public class GestoreLuciHue
    {
        // indirizzo del bridge Philips Hue: localhost:8000
        private readonly string _bridgeIp;
        // username per l'autenticazione: newdeveloper
        private readonly string _username;
        // client HTTP: client per la comunicazione HTTP
        private readonly HttpClient _httpClient;
        // URL per le luci: http://localhost:8000/api/newdeveloper/lights
        private readonly string _lightsURL;
        // logger: servizio di logging: ILogger
        private readonly ILogger _logger;

        // costruttore
        public GestoreLuciHue(string bridgeIp, string username, ILogger logger)
        {
            _bridgeIp = bridgeIp; // imposta l'indirizzo del bridge
            _username = username; // imposta l'username
            _httpClient = new HttpClient(); // inizializza il client HTTP
            _lightsURL = $"http://{_bridgeIp}/api/{_username}/lights"; // imposta l'URL per le luci
            _logger = logger; // inizializza il logger

        }

        // invia un comando al bridge Philips Hue
        internal async Task InviaComandoAsync(string luceId, string comando)
        {
            var url = $"{_lightsURL}/{luceId}/state"; // imposta l'URL per la luce specificata
            var content = new StringContent(comando, Encoding.UTF8, "application/json"); // imposta il contenuto della richiesta
            var response = await _httpClient.PutAsync(url, content); // invia la richiesta al bridge
            response.EnsureSuccessStatusCode(); // assicura che la richiesta sia andata a buon fine
        }

        // imposta il colore della luce a verde
        public async Task ImpostaColoreVerdeAsync(string luceId)
        {
            // comando per impostare il colore della luce a verde
            var comando = "{\"on\":true, \"hue\":25500, \"sat\":254, \"bri\":254}";
            // invia il comando al bridge Philips Hue
            await InviaComandoAsync(luceId, comando);
        }

        // imposta il colore della luce a rosso
        public async Task ImpostaColoreRossoAsync(string luceId)
        {
            // comando per impostare il colore della luce a rosso
            var comando = "{\"on\":true, \"hue\":0, \"sat\":254, \"bri\":254}";
            // invia il comando al bridge Philips Hue
            await InviaComandoAsync(luceId, comando);
        }

        // richiama lo stato della luce specificata
        public async Task RichiamaStatoLuce(string luceId)
        {
            // Assuming you have an API key or token
            string apiKey = "your_secret_key_here_your_secret_key_here";

            // richiesta HTTP per ottenere lo stato della luce specificata
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_lightsURL}/{luceId}");
            // aggiunge l'header Authorization con il token
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            // invia la richiesta al bridge Philips Hue
            var response = await _httpClient.SendAsync(request);
            // assicura che la richiesta sia andata a buon fine
            response.EnsureSuccessStatusCode();
            // legge il contenuto della risposta
            var content = await response.Content.ReadAsStringAsync();
            // se il contenuto contiene "error"
            if (content.Contains("error"))
            {
                // deserializza il contenuto in una lista di errori
                var errors = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content);
                // log: errore
                _logger.LogError($"Error light {luceId}: {JsonConvert.SerializeObject(errors, Formatting.Indented)}");
            }
            // altrimenti
            else
            {
                // deserializza il contenuto in un dizionario: stato della luce
                var light = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                // log: stato della luce
                _logger.LogInformation($"Light ID {luceId}:\n{JsonConvert.SerializeObject(light, Formatting.Indented)}");
            }
        }
    }
}
