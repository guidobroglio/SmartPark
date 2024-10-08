using Microsoft.Extensions.Logging;
using SmartParkCore.Models.Json;

namespace SmartParkManager.Gestori;

public class GestoreParcheggio : IGestoreParcheggio
{
    public const int MAXPOSTI = 4;
    readonly IRepositorySosta _repositorySosta;
    readonly IRepositoryPrenotazione _repositoryPrenotazione;
    readonly IRepositoryPosto _repositoryPosto;
    readonly IRepositoryVeicolo _repositoryVeicolo;
    readonly IRepositoryPagamento _repositoryPagamento;
    readonly IGestoreMonitor _gestoreMonitor;
    readonly ILogger<GestoreParcheggio> _logger;


    public GestoreParcheggio(IRepositorySosta repositorySosta, IRepositoryPrenotazione repositoryPrenotazione, IRepositoryPosto repositoryPosto,
        IGestoreMonitor gestoreSensori, IRepositoryVeicolo repositoryVeicolo, IRepositoryPagamento repositoryPagamento, ILogger<GestoreParcheggio> logger)
    {
        _repositorySosta = repositorySosta;
        _repositoryPrenotazione = repositoryPrenotazione;
        _repositoryPosto = repositoryPosto;
        _repositoryVeicolo = repositoryVeicolo;
        _gestoreMonitor = gestoreSensori;
        _repositoryPagamento = repositoryPagamento;
        _logger = logger;
    }

    public async Task<int> InserisciSosta(Sosta sosta)
    {
        // controlla che non ci siano altre soste attive per l'utente
        int numeroSoste = await _repositorySosta.ContaSosteInCorso(sosta.IdUtente);
        if (numeroSoste == 0)
        {
            var targa = await _repositoryVeicolo.RichiediTargaUtente(sosta.IdUtente);
            if (targa == sosta.Veicolo.Targa)
            {
                // inserisce la sosta
                await _repositorySosta.InserisciSosta(sosta);
                // aggiorna l'occupazione del posto
                await _repositoryPosto.ImpostaOccupato(sosta.Posto);
                await _gestoreMonitor.InviaOccupazioneParcheggio(sosta.Posto.Id, "non disponibile");
                return 0;
            }
            else
            {
                return 1; // targa del veicolo non associata ai veicoli dell'utente
            }
        }
        else
        {
            return 2; // utente già in sosta
        }
    }

    public async Task<int> InserisciSostaPrenotata(Sosta sosta)
    {
        var numeroPren = await _repositoryPrenotazione.RichiediPrenotazioniInSospeso(sosta.IdUtente);
        if (numeroPren.Count > 0)
        {
            if (numeroPren.First().DataFine >= sosta.DataInizio && numeroPren.First().DataInizio <= sosta.DataInizio)
            {
                _logger.LogInformation($"Gestore parcheggio: inserita sosta prenotata.");
                await _repositoryPrenotazione.CompletaPrenotazione(numeroPren.First());
                sosta.Posto.Id = await CalcolaPosto();
                return await InserisciSosta(sosta);
            }
            else if (numeroPren.First().DataFine <= sosta.DataInizio)
            {
                _logger.LogInformation($"Gestore parcheggio: addebitata penale per ritardo.");
                await _repositoryPrenotazione.CompletaPrenotazione(numeroPren.First());
                // crea e inserisce pagamento di penale per il ritardo
                var pagamento = ImpostaPenalePrenotazione(sosta);
                await _repositoryPagamento.InserisciPagamento(pagamento);
                return 3; // ritardo nella sosta, addebito penale
            }
            else
            {
                _logger.LogInformation($"Gestore parcheggio: utente in anticipo rispetto alla prenotazione.");
                return 4; // utente arrivato in anticipo rispetto alla prenotazione
            }
        }
        else
        {
            _logger.LogInformation($"Gestore parcheggio: nessuna prenotazione trovata.");
            return 5; // nessuna prenotazione
        }
    }

    private Pagamento ImpostaPenalePrenotazione(Sosta sosta)
    {
        var pagamento = new Pagamento
        {
            Id = Guid.NewGuid(),
            IdUtente = sosta.IdUtente,
            ImportoSosta = 5.0m,
            ImportoRicarica = 0,
            Data = sosta.DataFine,
            Veicolo = sosta.Veicolo
        };
        return pagamento;
    }

    public async Task<List<Sosta>> RichiediSosteUtente(Guid id)
    {
        var soste = await _repositorySosta.RichiediSosteUtente(id);
        _logger.LogInformation($"Gestore parcheggio: trovate {soste.Count} soste per l'utente {id}.");
        return soste;
    }

    public async Task<List<Sosta>> RichiediSoste()
    {
        var soste = await _repositorySosta.RichiediSosteUtente();
        _logger.LogInformation($"Gestore parcheggio: trovate {soste.Count} soste.");
        return soste;
    }

    public async Task<List<Sosta>> RichiediSosteInCorso(Guid id)
    {
        var soste = await _repositorySosta.RichiediSosteInCorso(id);
        _logger.LogInformation($"Gestore parcheggio: trovate {soste.Count} soste.");
        return soste;
    }

    public async Task<List<Sosta>> RichiediSosteInCorso()
    {
        var soste = await _repositorySosta.RichiediSosteInCorso();
        _logger.LogInformation($"Gestore parcheggio: trovate {soste.Count} soste.");
        return soste;
    }

    public async Task<IList<OccupazioneJson>> RichiediOccupazionePosti()
    {
        return await _repositoryPosto.RichiediOccupazionePosti();
    }

    public async Task<int> CalcolaPosto()
    {
        // conta i posti occupati
        var posti = await _repositoryPosto.PostiOccupati();
        var postiOccupati = new List<int>();
        foreach (var posto in posti)
        {
            postiOccupati.Add(posto.Id);
        }

        // conta i posti prenotati
        DateTime oraAttuale = DateTime.Now;
        int postiPrenotati = await _repositoryPrenotazione.RichiediPostiPrenotati(oraAttuale);

        int n = postiPrenotati + posti.Count;

        if (n < MAXPOSTI)
        {
            for (int i = 1; i <= MAXPOSTI; i++)
            {
                // restituisce il primo posto libero
                if (!postiOccupati.Contains(i))
                {
                    return i;
                }
            }
        }
        return -1;

    }
}
