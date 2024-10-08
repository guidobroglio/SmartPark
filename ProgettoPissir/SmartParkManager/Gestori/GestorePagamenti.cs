using Microsoft.Extensions.Logging;

namespace SmartParkManager.Gestori;

public class GestorePagamenti : IGestorePagamenti
{
    readonly IRepositoryPagamento _repositoryPagamento;
    readonly IRepositoryVeicolo _repositoryVeicolo;
    readonly IRepositoryRicarica _repositoryRicarica;
    readonly IRepositorySosta _repositorySosta;
    readonly IRepositoryPosto _repositoryPosto;
    readonly IRepositoryPrezzo _repositoryPrezzo;
    readonly IGestoreMonitor _gestoreMonitor;
    readonly ILogger<GestorePagamenti> _logger;


    public GestorePagamenti(IRepositoryVeicolo repositoryVeicolo, IRepositoryPagamento repositoryPagamento, IGestoreMonitor gestoreSensori, IRepositoryPrezzo repositoryPrezzo,
        IRepositoryRicarica repositoryRicarica, IRepositorySosta repositorySosta, ILogger<GestorePagamenti> logger, IRepositoryPosto repositoryPosto)
    {
        _repositoryVeicolo = repositoryVeicolo;
        _repositoryPagamento = repositoryPagamento;
        _gestoreMonitor = gestoreSensori;
        _repositoryRicarica = repositoryRicarica;
        _repositorySosta = repositorySosta;
        _repositoryPosto = repositoryPosto;
        _repositoryPrezzo = repositoryPrezzo;
        _logger = logger;
    }

    public async Task<List<Pagamento>> RichiediPagamenti()
    {
        var pagamenti = await _repositoryPagamento.RichiediPagamenti();
        _logger.LogInformation($"Gestore pagamenti: trovati {pagamenti.Count} pagamenti.");
        return pagamenti;
    }

    public async Task<List<Pagamento>> RichiediPagamentiUtente(Guid id)
    {
        var pagamenti = await _repositoryPagamento.RichiediPagamentiUtente(id);
        _logger.LogInformation($"Gestore pagamenti: trovati {pagamenti.Count} pagamenti.");
        return pagamenti;
    }

    public async Task<int> InserisciPagamento(string targa)
    {
        // cerca la sosta corrispondende al veicolo
        var sosta = await _repositorySosta.RichiediSostaInCorsoVeicolo(targa);

        if (sosta != null)
        {
            var dataFine = DateTime.Now;

            // calcola l'importo totale delle eventuali ricariche
            decimal importoRicarica = await CalcolaImportoRicariche(sosta.IdUtente, sosta.DataInizio, dataFine);

            // calcola l'importo della sosta appena conclusa
            decimal importoSosta = await CalcolaImportoSosta(sosta.DataInizio, dataFine);
            _logger.LogInformation($"Gestore pagamenti: pagamento con importo ricarica = {importoRicarica}, importo sosta = {importoSosta}");

            // crea pagamento
            var pagamento = new Pagamento
            {
                Id = Guid.NewGuid(),
                IdUtente = sosta.IdUtente,
                ImportoSosta = importoSosta,
                ImportoRicarica = importoRicarica,
                Data = sosta.DataFine,
                Veicolo = sosta.Veicolo
            };

            sosta.Durata = (int)(dataFine - sosta.DataInizio).TotalMinutes;
            sosta.PrezzoSosta = Math.Round(importoSosta, 2);
            sosta.DataFine = dataFine;

            // libera il posto del parcheggio
            await _repositoryPosto.ImpostaDisponibile(sosta.Posto);
            await _gestoreMonitor.InviaOccupazioneParcheggio(sosta.Posto.Id, "disponibile");

            // termina la sosta e crea il relativo pagamento
            await _repositorySosta.InserisciSostaCompletata(sosta);
            await _repositoryPagamento.InserisciPagamento(pagamento);

            return 0;
        }
        else
        {
            return 1;
        }
    }

    private async Task<decimal> CalcolaImportoRicariche(Guid idUtente, DateTime dataInizio, DateTime dataFine)
    {
        // cerca eventuali ricariche effettuate durante la sosta
        var ricariche = await _repositoryRicarica.RichiediRicaricheDaPagare(idUtente, dataInizio, dataFine);

        decimal importoRicarica = 0;
        foreach (var ricarica in ricariche)
        {
            importoRicarica += ricarica.PrezzoRicarica;
        }

        return importoRicarica;
    }

    private async Task<decimal> CalcolaImportoSosta(DateTime dataInizio, DateTime dataFine)
    {
        var prezzo = await _repositoryPrezzo.PrezzoSosta();
        return Math.Round((decimal)(dataFine - dataInizio).TotalMinutes * prezzo, 2);
    }
}
