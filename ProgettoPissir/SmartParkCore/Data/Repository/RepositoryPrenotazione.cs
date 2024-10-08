using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SmartParkCore.Data.Repository;

public class RepositoryPrenotazione : IRepositoryPrenotazione
{
    readonly DataContext _context;
    readonly ILogger<RepositoryPrenotazione> _logger;

    public RepositoryPrenotazione(DataContext context, ILogger<RepositoryPrenotazione> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> RichiediPostiPrenotati(DateTime data)
    {
        var numeroPosti = await _context.Prenotazioni
            .Where(p => p.DataInizio <= data && p.DataFine >= data && !p.Stato)
            .CountAsync();

        _logger.LogInformation($"Repository prenotazione: trovati {numeroPosti} posti occupati.");

        return numeroPosti;
    }

    public async Task<List<Prenotazione>> RichiediPrenotazioniValide(Guid idUtente, DateTime data)
    {
        var prenotazioni = await _context.Prenotazioni
            .Where(p => p.IdUtente == idUtente && p.DataInizio <= data && p.DataFine >= data && !p.Stato)
            .ToListAsync();

        _logger.LogInformation($"Repository prenotazione: trovate {prenotazioni.Count} prenotazioni valide.");

        return prenotazioni;
    }

    public async Task<List<Prenotazione>> RichiediPrenotazioniInSospeso(Guid idUtente)
    {
        var prenotazioni = await _context.Prenotazioni
            .Where(p => p.IdUtente == idUtente && !p.Stato)
            .ToListAsync();

        _logger.LogInformation($"Repository prenotazione: trovate {prenotazioni.Count} prenotazioni valide.");

        return prenotazioni;
    }

    public async Task<List<Prenotazione>> RichiediPrenotazioniInSospeso()
    {
        var prenotazioni = await _context.Prenotazioni
            .Where(p => !p.Stato)
            .ToListAsync();

        _logger.LogInformation($"Repository prenotazione: trovate {prenotazioni.Count} prenotazioni valide.");

        return prenotazioni;
    }

    public async Task InserisciPrenotazione(Prenotazione prenotazione)
    {
        await _context.Prenotazioni.AddAsync(prenotazione);
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Repository prenotazione: inserita prenotazione per l'orario {prenotazione.DataInizio} - {prenotazione.DataFine}.");
        }
        catch (Exception e)
        {
            _logger.LogError($"Repository prenotazione: {e.Message}");
        }
    }

    public async Task CompletaPrenotazione(Prenotazione prenotazione)
    {
        await _context.Prenotazioni
            .Where(p => p.Id == prenotazione.Id)
            .ExecuteUpdateAsync(modifiche => modifiche
                .SetProperty(p => p.Stato, true));
    }
}
