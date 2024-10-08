using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartParkCore.Models.Entities;
using SmartParkCore.Models.Json;

namespace SmartParkCore.Data.Repository;

public class RepositoryPosto : IRepositoryPosto
{
    readonly DataContext _context;
    readonly ILogger<RepositoryPosto> _logger;

    public RepositoryPosto(DataContext context, ILogger<RepositoryPosto> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ImpostaOccupato(Posto posto)
    {
        await _context.Posti.Where(p => p.Id == posto.Id)
            .ExecuteUpdateAsync(modifiche => modifiche
            .SetProperty(p => p.Disponibile, false));
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Repository posto: impostato occupato il posto {posto.Id}.");
        }
        catch (Exception e)
        {
            _logger.LogError($"Repository posto: {e.Message}");
        }
    }

    public async Task ImpostaDisponibile(Posto posto)
    {
        await _context.Posti.Where(p => p.Id == posto.Id)
            .ExecuteUpdateAsync(modifiche => modifiche
            .SetProperty(p => p.Disponibile, true));
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Repository posto: impostato disponibile il posto {posto.Id}.");
        }
        catch (Exception e)
        {
            _logger.LogError($"Repository posto: {e.Message}");
        }
    }

    public async Task<IList<Posto>> PostiOccupati()
    {
        var posti = await _context.Posti
            .Where(p => p.Disponibile == false)
            .ToListAsync();

        _logger.LogInformation($"Repository posto: trovati {posti.Count} posti occupati.");

        return posti;
    }

    public async Task<IList<OccupazioneJson>> RichiediOccupazionePosti()
    {
        var occupazione = new OccupazioneJson[4];

        var posti = await _context.Posti.ToListAsync();
        _logger.LogInformation($"Repository posto: restituiti tutti i posti.");
        var soste = await _context.Soste
            .Include(s => s.Posto)
            .Include(s => s.Veicolo)
            .Where(s => s.PrezzoSosta == 0)
            .ToListAsync();
        _logger.LogInformation($"Repository posto: restituite {soste.Count} soste in corso.");

        for (int i = 0; i < 4; i++)
        {
            occupazione[i] = new OccupazioneJson
            {
                Id = posti[i].Id,
                Disponibile = posti[i].Disponibile
            };
            if (soste.Count > 0)
            {
                foreach (var sosta in soste)
                {
                    if (sosta.Posto.Id == posti[i].Id)
                    {
                        occupazione[i].Targa = sosta.Veicolo.Targa;
                        occupazione[i].DataInizio = sosta.DataInizio;
                    }
                }
            }
        }

        return occupazione;
    }
}
