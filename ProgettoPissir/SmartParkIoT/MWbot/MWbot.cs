using MQTTnet.Protocol;
using System.Text;

namespace SmartParkIoT.MWbot;

// MWbot è un servizio che si occupa della gestione del robot per la ricarica dei veicoli elettrici
public class MWbot
{
    // logger: servizio di logging
    readonly ILogger<MWbot> _logger;
    // client MQTT: client per la comunicazione MQTT
    readonly IMqttClient _mqttClient;

    // costruttore
    public MWbot(ILogger<MWbot> logger)
    {
        // inizializza il client MQTT
        _mqttClient = new MqttFactory().CreateMqttClient();
        // inizializza il logger
        _logger = logger;
    }

    // avvia il servizio MWbot
    public async Task AvviaMWbot()
    {
        // tenta di connettersi al server MQTT
        try
        {
            // imposta le opzioni per il client MQTT
            var options = new MqttClientOptionsBuilder() // costruttore di opzioni per il client MQTT
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
                _logger.LogInformation("MWbot attivato.");
                // sottoscrizione al topic per la ricezione delle richieste di ricarica
                await _mqttClient.SubscribeAsync(Topics.MWbotInizia());
                // log
                _logger.LogInformation($"MWbot sottoscritto al topic {Topics.MWbotInizia()}.");
                // gestione dei messaggi ricevuti
                _mqttClient.ApplicationMessageReceivedAsync += async e =>
                {
                    // messaggio ricevuto
                    var message = e.ApplicationMessage;
                    // payload del messaggio
                    var payload = Encoding.UTF8.GetString(message.PayloadSegment);
                    // se il payload non è nullo e il topic contiene "inizia_ricarica"
                    if (payload != null && message.Topic.ToLowerInvariant().Contains("inizia_ricarica"))
                    {
                        // deserializza il payload in un oggetto RicaricaJson
                        var ricarica = JsonSerializer.Deserialize<RicaricaJson>(payload);
                        // log
                        _logger.LogInformation($"MWbot: ricevuta richiesta di ricarica dal {ricarica.Veicolo.PercentualeBatteria}% al {ricarica.PercentualeRicarica}% per il posto {ricarica.IdPosto}.");
                        // esegui la ricarica
                        await EseguiRicarica(ricarica);
                    }
                    // altrimenti
                    else
                    {
                        // log
                        _logger.LogError("MWbot: errore nella ricezione.");
                    }
                };
            }
        }
        // in caso di eccezione
        catch (Exception e)
        {
            // log
            _logger.LogError($"MWbot: {e.Message}");
        }
    }

    // esegue la ricarica del veicolo
    private async Task EseguiRicarica(RicaricaJson ricarica)
    {
        //simula massimo un minuto di attesa per il completamento della ricarica
        var tempoRicarica = (ricarica.PercentualeRicarica-ricarica.Veicolo.PercentualeBatteria) * 600;
        //log
        _logger.LogInformation($"MWbot: avvio carica di durata {tempoRicarica/1000}.");

        //invia messaggio su mwbot/status per indicare che è occupato
        var mexMWbotOccupato = GeneraMessaggioMqtt(Topics.MWbotStatus(), "occupato");
        //invia messaggio su mwbot/status per indicare che è occupato
        await _mqttClient.PublishAsync(mexMWbotOccupato);
        //registra l'inizio della ricarica
        ricarica.DataInizio = DateTime.Now;  
        //sospendi per tempoRicarica
        await Task.Delay(tempoRicarica);

        //registra la fine della ricarica
        ricarica.DataFine = DateTime.Now;

        //calcola il prezzo della ricarica
        var prezzo = (ricarica.PercentualeRicarica - ricarica.Veicolo.PercentualeBatteria) * ricarica.Veicolo.CapacitaBatteria * 0.0047;
        //aggiorna il prezzo della
        ricarica.PrezzoRicarica = (decimal)Math.Round(prezzo, 2);

        //aggiorna la batteria del veicolo che è stato ricaricato
        ricarica.Veicolo.PercentualeBatteria = ricarica.PercentualeRicarica;
        //serializza l'oggetto ricarica
        var ricaricaCompletata = JsonSerializer.Serialize(ricarica);

        //invia messaggio su mwbot/<n>/ricarica_completata
        var messaggio = GeneraMessaggioMqtt(Topics.MWbotRicaricaCompletata(ricarica.IdPosto), ricaricaCompletata);
        //invia messaggio
        await _mqttClient.PublishAsync(messaggio);

        //invia messaggio su mwbot/status per indicare che è libero
        var mexMWbotLibero = GeneraMessaggioMqtt(Topics.MWbotStatus(), "libero");
        //invia messaggio
        await _mqttClient.PublishAsync(mexMWbotLibero);
    }

    //genera un messaggio MQTT
    private MqttApplicationMessage GeneraMessaggioMqtt(string topic, string payload)
    {
        //costruisce il messaggio
        var messaggio = new MqttApplicationMessageBuilder() //costruttore di messaggi MQTT
                   .WithTopic(topic)  //imposta il topic
                   .WithPayload(payload) //imposta il payload
                   .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce) //imposta la qualità del servizio
                   .Build(); //costruisce il messaggio
        return messaggio; //restituisce il messaggio
    }
}
