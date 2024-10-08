using MQTTnet.Protocol;
using SmartParkCore.Models.Entities;
using SmartParkPhilipsHue;
using SmartParkPhilipsHue.Core;
using System.Text;

namespace SmartParkIoT.Parcheggio;

public class MonitorPosti
{
    // MonitorPosti è un servizio che si occupa di monitorare l'occupazione dei posti del parcheggio
    readonly ILogger<MonitorPosti> _logger;
    // client MQTT
    readonly IMqttClient _mqttClient;
    // repository per i posti
    readonly IRepositoryPosto _repositoryPosto;
    // gestore delle luci Philips Hue
    readonly GestoreLuciHue _hueLightManager;

    // costruttore
    public MonitorPosti(ILogger<MonitorPosti> logger, IRepositoryPosto repositoryPosto)
    {
        // inizializza il client MQTT
        _mqttClient = new MqttFactory().CreateMqttClient();
        // inizializza il logger
        _logger = logger;
        // inizializza il repository dei posti
        _repositoryPosto = repositoryPosto;
        // inizializza il gestore delle luci Philips Hue
        _hueLightManager = new GestoreLuciHue("localhost:8000", "newdeveloper", logger);
    }

    // avvia il monitoraggio dei posti
    public async Task AvviaMonitorPosti()
    {
        // avvia il monitoraggio delle luci Philips Hue
        await AvviaPhilipsHue();

        // imposta le opzioni per il client MQTT
        var options = new MqttClientOptionsBuilder() 
            .WithTcpServer("localhost", 1883) // indirizzo del server MQTT
            .WithClientId(Guid.NewGuid().ToString()) // identificativo del client
            .WithCleanSession() // pulizia della sessione
            .Build(); // costruisce le opzioni

        // connessione al server MQTT
        var connesso = await _mqttClient.ConnectAsync(options);

        // se la connessione è avvenuta con successo
        if (connesso.ResultCode == MqttClientConnectResultCode.Success)
        {
            // log
            _logger.LogInformation("Monitor posti attivato.");
            // sottoscrizione al topic per il monitoraggio dei posti
            await _mqttClient.SubscribeAsync(Topics.ParcheggioOccupazione());
            // log
            _logger.LogInformation($"Monitor posti sottoscritto al topic {Topics.ParcheggioOccupazione()}.");
            // gestione dei messaggi ricevuti
            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                // gestione del messaggio
                await GestisciMessaggio(e.ApplicationMessage);
            };
        }
    }

    // gestione dei messaggi ricevuti
    private async Task GestisciMessaggio(MqttApplicationMessage messaggio)
    {
        // log
        _logger.LogInformation($"Monitor posti: ricevuto messaggio.");
        // estrazione del payload
        var disponibile = Encoding.UTF8.GetString(messaggio.PayloadSegment);
        // se il messaggio contiene la disponibilità di un posto e il topic è relativo al parcheggio
        if (disponibile != null && messaggio.Topic.ToLowerInvariant().Contains("parcheggio"))
        {
            // estrazione del posto
            var posto = Topics.EstraiPostoParcheggio(messaggio.Topic);
            // log
            _logger.LogInformation($"Monitor posti: posto {posto} {disponibile} sul topic {Topics.ParcheggioOccupazione()}.");
            // se il posto è disponibile
            if (disponibile == "disponibile")
            {
                // lampadina verde (veicolo in uscita)                
                await _hueLightManager.ImpostaColoreVerdeAsync(posto);
            }
            else
            {
                // lampadina rossa (veicolo in ingresso)
                await _hueLightManager.ImpostaColoreRossoAsync(posto);
            }
        }
        else
        {
            // log
            _logger.LogError($"Monitor posti: errore nella ricezione per il topic {Topics.ParcheggioOccupazione}.");
        }
    }

    // avvia il monitoraggio delle luci Philips Hue
    private async Task AvviaPhilipsHue()
    {
        // richiedi l'occupazione dei posti
        var posti = await _repositoryPosto.RichiediOccupazionePosti();
        // imposta l'emulatore delle lampadine (che rappresenta il monitor dei posti)
        foreach (var posto in posti)
        {
            // se il posto è disponibile
            if (posto.Disponibile)
            {
                // lampadina verde
                await _hueLightManager.ImpostaColoreVerdeAsync(posto.Id.ToString());
            }
            else
            {
                // lampadina rossa
                await _hueLightManager.ImpostaColoreRossoAsync(posto.Id.ToString());
            }
        }
    }
}
