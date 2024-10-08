using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartParkCore.Models.Entities;

namespace SmartParkCore.Data.Repository;

public class RepositoryRicarica : IRepositoryRicarica
{
    readonly DataContext _context;
    readonly ILogger<RepositoryRicarica> _logger;

    public RepositoryRicarica(DataContext context, ILogger<RepositoryRicarica> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Ricarica>> RichiediRicaricheUtente(Guid id)
    {
        return await _context.Ricariche
            .Include(r => r.Veicolo)
            .Where(ricarica => (ricarica.IdUtente == id && ricarica.Completata == true))
            .ToListAsync();
    }

    public async Task<List<Ricarica>> RichiediRicariche()
    {
        return await _context.Ricariche
            .Include(r => r.Veicolo)
            .Where(ricarica => (ricarica.Completata == true))
            .ToListAsync();
    }

    public async Task<Ricarica> RichiediRicaricaInCorso(Guid id)
    {
        var ricarica = await _context.Ricariche
            .Include(r => r.Veicolo)
            .Where(ricarica => (ricarica.IdUtente == id && !ricarica.Completata))
            .FirstOrDefaultAsync();
        if (ricarica != null)
        {
            _logger.LogInformation($"Repository ricarica: trovata ricarica in corso, richiesta alle {ricarica.DataInizio.ToString("HH:mm:ss")}.");
            return ricarica;
        }
        else
        {
            _logger.LogInformation($"Repository ricarica: nessuna ricarica in corso.");
            return null;
        }
    }

    public async Task<List<Ricarica>> RichiediRicaricheDaPagare(Guid id, DateTime dataInizio, DateTime dataFine)
    {
        return await _context.Ricariche
            .Where(ricarica => (ricarica.IdUtente == id && ricarica.Completata == true
                && dataInizio <= ricarica.DataInizio && dataFine >= ricarica.DataFine))
            .ToListAsync();
    }

    public async Task InserisciRicaricaCompletata(Ricarica nuovaRicarica)
    {
        await _context.Veicoli
            .Where(v => v.Id == nuovaRicarica.Veicolo.Id)
            .ExecuteUpdateAsync(modifiche => modifiche
            .SetProperty(v => v.PercentualeBatteria, nuovaRicarica.Veicolo.PercentualeBatteria));

        await _context.Ricariche
            .Where(r => r.Id == nuovaRicarica.Id)
            .ExecuteUpdateAsync(modifiche => modifiche
                .SetProperty(r => r.PrezzoRicarica, nuovaRicarica.PrezzoRicarica)
                .SetProperty(r => r.Completata, nuovaRicarica.Completata)
                .SetProperty(r => r.DataInizio, nuovaRicarica.DataInizio)
                .SetProperty(r => r.DataFine, nuovaRicarica.DataFine));
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Repository ricarica: ricarica completata salvata sul DataBase.");
        }
        catch (Exception e)
        {
            _logger.LogError($"Repository ricarica: {e.Message}");
        }
    }


    public async Task<Ricarica> ProssimaRicarica()
    {
        var ricarica = await _context.Ricariche
           .Include(r => r.Veicolo)
           .Where(r => r.Completata == false)
           .OrderBy(r => r.DataInizio)
           .FirstOrDefaultAsync();

        if (ricarica != null)
        {
            _logger.LogInformation($"Repository ricarica: prossima ricarica, richiesta alle {ricarica.DataInizio.ToString("HH:mm:ss")}.");
            return ricarica;
        }
        else
        {
            _logger.LogInformation($"Repository ricarica: nessuna ricarica in coda.");
            return null;
        }
    }

    public async Task<int> RichiediRicaricheInCoda()
    {
        var numeroRicariche = await _context.Ricariche
           .Where(r => !r.Completata)
           .CountAsync();

        return numeroRicariche;
    }

    public async Task InserisciRichiestaRicarica(Ricarica ricarica)
    {
        var percentualeBatteria = ricarica.Veicolo.PercentualeBatteria;

        var veicolo = await _context.Veicoli.Where(v => v.Targa == ricarica.Veicolo.Targa)
            .FirstOrDefaultAsync();
        if (veicolo != null)
        {
            ricarica.Veicolo = veicolo;
            ricarica.Veicolo.PercentualeBatteria = percentualeBatteria;
        }

        await _context.Ricariche.AddAsync(ricarica);
        await _context.SaveChangesAsync();
    }
}
