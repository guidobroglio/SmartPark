using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace SmartParkManager.Gestori;

public class GestoreSensori : IGestoreMonitor
{
    readonly IRepositorySosta _repositorySosta;
    readonly IRepositoryVeicolo _repositoryVeicolo;
    readonly IRepositoryPosto _repositoryPosto;
    readonly IMqttClient _mqttClient;
    readonly ILogger<GestoreSensori> _logger;

    public GestoreSensori(IRepositorySosta repositorySosta, IRepositoryVeicolo repositoryVeicolo,
        IRepositoryPosto repositoryPosto, ILogger<GestoreSensori> logger)
    {
        _repositorySosta = repositorySosta;
        _repositoryVeicolo = repositoryVeicolo;
        _repositoryPosto = repositoryPosto;
        _logger = logger;
        _mqttClient = new MqttFactory().CreateMqttClient();
    }

    public async Task InviaOccupazioneParcheggio(int posto, string disponibile)
    {
        var esito = await Connetti();
        if (esito == MqttClientConnectResultCode.Success)
        {
            _logger.LogInformation($"Gestore sensori: invio messaggio di occupazione del posto {posto}.");
            var messaggio = new MqttApplicationMessageBuilder()
                   .WithTopic(Topics.ParcheggioOccupazione(posto))
                   .WithPayload(disponibile)
                   .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                   .Build();
            await _mqttClient.PublishAsync(messaggio);
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
}
