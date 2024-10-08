using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SmartParkCore.Data.Repository;

public class RepositorySosta : IRepositorySosta
{
    readonly DataContext _context;
    readonly ILogger<RepositorySosta> _logger;

    public RepositorySosta(DataContext context, ILogger<RepositorySosta> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InserisciSosta(Sosta sosta)
    {
        var posto = await _context.Posti.Where(p => p.Id == sosta.Posto.Id)
            .FirstAsync();
        var veicolo = await _context.Veicoli.Where(v => v.Targa == sosta.Veicolo.Targa)
            .FirstAsync();
        if (posto != null)
        {
            sosta.Posto = posto;
            sosta.Veicolo = veicolo;
            _logger.LogInformation($"Repository sosta: inserimento sosta al posto {posto.Id}.");
        }

        await _context.Soste.AddAsync(sosta);
        await _context.SaveChangesAsync();
    }

    public async Task<int> ContaSosteInCorso(Guid id)
    {
        var numeroSoste = await _context.Soste.Where(s => s.IdUtente == id && s.PrezzoSosta == 0)
            .CountAsync();
        return numeroSoste;
    }

    public async Task<List<Sosta>> RichiediSosteInCorso(Guid id)
    {
        var soste = await _context.Soste
            .Include(s => s.Posto)
            .Include(s => s.Veicolo)
            .Where(s => s.IdUtente == id && s.PrezzoSosta == 0)
            .ToListAsync();
        
        return soste;
    }

    public async Task<List<Sosta>> RichiediSosteInCorso()
    {
        var soste = await _context.Soste
            .Include(s => s.Posto)
            .Include(s => s.Veicolo)
            .Where(s => s.PrezzoSosta == 0)
            .ToListAsync();

        return soste;
    }

    public async Task<Sosta> RichiediSostaInCorsoVeicolo(string targa)
    {
        var veicolo = await _context.Veicoli.Where(v => v.Targa == targa).FirstAsync();
        var sosta = await _context.Soste
            .Include(s => s.Posto)
            .Where(s => s.Veicolo.Id == veicolo.Id && s.PrezzoSosta == 0)
            .FirstOrDefaultAsync();
        if (sosta != null)
        {
            _logger.LogInformation($"Repository sosta: trovata sosta in corso, richiesta alle {sosta.DataInizio.ToString("HH:mm:ss")}.");
            return sosta;
        }
        else
        {
            _logger.LogInformation($"Repository sosta: nessuna sosta in corso.");
            return null;
        }
    }

    public async Task InserisciSostaCompletata(Sosta sosta)
    {
        await _context.Soste
            .Where(s => s.Id == sosta.Id)
            .ExecuteUpdateAsync(modifiche => modifiche
                .SetProperty(s => s.PrezzoSosta, sosta.PrezzoSosta)
                .SetProperty(s => s.DataFine, sosta.DataFine)
                .SetProperty(s => s.Durata, sosta.Durata));
        await _context.SaveChangesAsync();
    }

    public async Task<List<Sosta>> RichiediSosteUtente(Guid id)
    {
        return await _context.Soste
            .Include(s => s.Posto)
            .Include(s => s.Veicolo)
            .Where(sosta => sosta.IdUtente == id)
            .ToListAsync();
    }

    public async Task<List<Sosta>> RichiediSosteUtente()
    {
        return await _context.Soste
            .Include(s => s.Posto)
            .Include(s => s.Veicolo)
            .ToListAsync();
    }
}
