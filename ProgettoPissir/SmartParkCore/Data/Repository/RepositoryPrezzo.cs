using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SmartParkCore.Data.Repository;

public class RepositoryPrezzo : IRepositoryPrezzo
{
    readonly DataContext _context;
    readonly ILogger<RepositoryPrezzo> _logger;

    public RepositoryPrezzo(DataContext context, ILogger<RepositoryPrezzo> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<decimal> PrezzoSosta()
    {
        var prezzo = await _context.Prezzi
            .Where(p => p.Id == 1)
            .FirstAsync();

        _logger.LogInformation($"Repository prezzo: restituito il prezzo della sosta: {prezzo.PrezzoSosta}.");

        return prezzo.PrezzoSosta;
    }

    public async Task<decimal> PrezzoRicarica()
    {
        var prezzo = await _context.Prezzi
            .Where(p => p.Id == 1)
            .FirstAsync();

        _logger.LogInformation($"Repository prezzo: restituito il prezzo della sosta: {prezzo.PrezzoRicarica}.");

        return prezzo.PrezzoRicarica;
    }

    public async Task ModificaPrezzi(Prezzo prezzo)
    {
        await _context.Prezzi
            .Where(p => p.Id == 1)
            .ExecuteUpdateAsync(modifiche => modifiche
                .SetProperty(p => p.PrezzoSosta, prezzo.PrezzoSosta)
                .SetProperty(p => p.PrezzoRicarica, prezzo.PrezzoRicarica));
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Repository prezzo: aggiornato prezzo sosta sul DataBase.");
        }
        catch (Exception e)
        {
            _logger.LogError($"Repository prezzo: {e.Message}");
        }
    }

    public async Task<Prezzo> RichiediPrezzi()
    {
        var prezzo = await _context.Prezzi
            .Where(p => p.Id == 1).FirstAsync();
        _logger.LogInformation("Repository prezzo: restituiti prezzi sosta e ricarica.");
        return prezzo;
    }
}
