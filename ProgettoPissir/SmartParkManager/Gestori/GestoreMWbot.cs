using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;
using SmartParkCore.Models.Json;
using SmartParkCore.Models.Json.Converters;
using MQTTnet.Protocol;
using Microsoft.AspNetCore.SignalR;
using SmartParkManager.Hubs;

namespace SmartParkManager.Gestori;

public class GestoreMWbot : IGestoreMWbot
{
    readonly IRepositoryRicarica _repositoryRicarica;
    readonly IRepositorySosta _repositorySosta;
    readonly IMqttClient _mqttClient;
    readonly ILogger<GestoreMWbot> _logger;
    readonly IHubContext<NotificaRicarica> _hubContext;

    public GestoreMWbot(IRepositoryRicarica repositoryRicarica, IRepositorySosta repositorySosta,
        ILogger<GestoreMWbot> logger, IHubContext<NotificaRicarica> hubContext)
    {
        _repositoryRicarica = repositoryRicarica;
        _logger = logger;
        _repositorySosta = repositorySosta;
        _hubContext = hubContext;
        _mqttClient = new MqttFactory().CreateMqttClient();
    }

    public async Task AvviaGestoreRicariche()
    {
        try
        {
            var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883)
            .WithClientId(Guid.NewGuid().ToString())
            .WithCleanSession()
            .Build();

            var connesso = await _mqttClient.ConnectAsync(options);

            if (connesso.ResultCode == MqttClientConnectResultCode.Success)
            {
                _logger.LogInformation("Gestore MWbot attivato.");
                await _mqttClient.SubscribeAsync(Topics.MWbotRicaricaCompletata());
                _logger.LogInformation($"Gestore MWbot sottoscritto al topic {Topics.MWbotRicaricaCompletata()}.");
                await _mqttClient.SubscribeAsync(Topics.MWbotStatus());
                _logger.LogInformation($"Gestore MWbot sottoscritto al topic {Topics.MWbotStatus()}.");
                _mqttClient.ApplicationMessageReceivedAsync += async e =>
                {
                    await GestisciMessaggio(e.ApplicationMessage);
                };
            }

            if (_mqttClient.IsConnected)
            {
                _logger.LogInformation("Gestore MWbot attivato.");
            }
            else
            {
                _logger.LogError("Gestore MWbot non è riuscito a connettersi al broker MQTT.");
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Gestore MWbot: {e.Message}");
        }
    }
    
    private async Task GestisciMessaggio(MqttApplicationMessage message)
    {
        var payload = Encoding.UTF8.GetString(message.PayloadSegment);

        if (payload != null && message.Topic.ToLowerInvariant().Contains("ricarica_completata"))
        {
            var ricaricaJson = JsonSerializer.Deserialize<RicaricaJson>(payload);
            _logger.LogInformation($"Gestore MWbot: ricevuto messaggio sul topic {message.Topic}, per ricarica {payload}.");

            var ricarica = ConvertiRicarica.Converti(ricaricaJson);

            ricarica.Completata = true;
            await _repositoryRicarica.InserisciRicaricaCompletata(ricarica);

            // invia notifica di ricarica completata
            await _hubContext.Clients.All.SendAsync("RicaricaCompletata", ricarica.IdUtente.ToString(), JsonSerializer.Serialize(ricaricaJson));

            // ricariche mancanti
            var ricariche = await _repositoryRicarica.RichiediRicaricheInCoda();

            if (ricariche == 1)
            {
                _logger.LogInformation($"Gestore MWbot: {ricariche} in coda.");
                var prossimaRicarica = await _repositoryRicarica.ProssimaRicarica();
                if (prossimaRicarica != null)
                {
                    _logger.LogInformation("Gestore MWbot: avvio della prossima ricarica.");
                    await IniziaRicarica(prossimaRicarica);
                }
                else
                {
                    _logger.LogInformation("Gestore MWbot: nessuna ricarica in coda.");
                }
            }
            else
            {
                _logger.LogInformation($"Gestore MWbot: {ricariche} in coda.");
            }
        }
        else if (payload != null && message.Topic.ToLowerInvariant().Contains("status"))
        {
            if (payload == "occupato")
            {
                _logger.LogInformation("MWbot occupato.");
            }
            else if (payload == "libero")
            {
                _logger.LogInformation("MWbot libero.");
            }
        }
        else
        {
            _logger.LogError($"Gestore MWbot: errore, ricevuto messaggio sul topic {message.Topic}.");
        }
    }

    public async Task<int> RichiediRicarica(Ricarica ricarica)
    {
        // controlla che l'utente sia in sosta quando chiede la ricarica
        var soste = await _repositorySosta.RichiediSosteInCorso(ricarica.IdUtente);

        if (soste.Count == 1 )
        {
            var sosta = soste.First();
            ricarica.Veicolo.Targa = sosta.Veicolo.Targa;
            ricarica.IdPosto = sosta.Posto.Id;
            // inserisce ricarica da completare
            await _repositoryRicarica.InserisciRichiestaRicarica(ricarica);

            var ricariche = await _repositoryRicarica.RichiediRicaricheInCoda();
            if (ricariche == 1)
            {
                _logger.LogInformation("Gestore MWbot: avvio ricarica.");
                var esito = await Connetti();
                if (esito == MqttClientConnectResultCode.Success)
                {
                    _logger.LogInformation($"Gestore MWbot: avviata nuova ricarica dal {ricarica.Veicolo.PercentualeBatteria}% al {ricarica.PercentualeRicarica}% per il posto {ricarica.IdPosto}.");
                   
                    await IniziaRicarica(ricarica);
                }
                return 0;
            }
            else
            {
                _logger.LogInformation($"Gestore MWbot: {ricariche} in coda, ricarica corrente in attesa.");
                return 1; // mwbot occupato
            }
        }
        else
        {
            return 2; // veicolo non in sosta
        }
    }

    private async Task<MqttClientConnectResultCode> Connetti()
    {
        var options = new MqttClientOptionsBuilder()
           .WithTcpServer("localhost", 1883)
           .WithClientId(Guid.NewGuid().ToString())
           .WithCleanSession()
           .Build();

        var connesso = await _mqttClient.ConnectAsync(options);

        return connesso.ResultCode;
    }

    private async Task IniziaRicarica(Ricarica ricarica)
    {
        var ricaricaJson = ConvertiRicarica.Converti(ricarica);
        var messaggio = new MqttApplicationMessageBuilder()
                .WithTopic(Topics.MWbotInizia(ricarica.IdPosto))
                .WithPayload(JsonSerializer.Serialize(ricaricaJson))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

        await _mqttClient.PublishAsync(messaggio);
    }
}
