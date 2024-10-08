using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartParkCore.Models.Entities;

namespace SmartParkCore.Data.Repository;

public class RepositoryVeicolo : IRepositoryVeicolo
{
    readonly DataContext _context;
    readonly ILogger<RepositoryVeicolo> _logger;    

    public RepositoryVeicolo(DataContext context, ILogger<RepositoryVeicolo> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Guid> RichiediUtenteTarga(string targa)
    {
        var veicolo = await _context.Veicoli.Where(v => v.Targa == targa).FirstAsync();

        var utente = await _context.Utenti
            .Where(u => u.Id == veicolo.IdUtente).FirstAsync();

        return utente.Id;
    }

    public async Task<string> RichiediTargaUtente(Guid id)
    {
        var veicolo = await _context.Veicoli
           .Where(v => v.IdUtente == id).FirstAsync();

        _logger.LogInformation($"Repository veicolo: trovato veicolo dell'utente {id}");

        return veicolo.Targa;
    }

    public async Task InserisciVeicolo(Veicolo veicolo)
    {
        await _context.Veicoli.AddAsync(veicolo);
        await _context.SaveChangesAsync();
    }

    public async Task ModificaVeicolo(Veicolo veicolo)
    {
        _logger.LogInformation($"Repository veicolo: aggiornamento del veicolo {veicolo.Id} {veicolo.Targa} {veicolo.CapacitaBatteria}");

        await _context.Veicoli
            .Where(v => v.Id == veicolo.Id)
            .ExecuteUpdateAsync(modifiche => modifiche
                .SetProperty(v => v.Targa, veicolo.Targa)
                .SetProperty(v => v.CapacitaBatteria, veicolo.CapacitaBatteria));
        await _context.SaveChangesAsync();
    }

    public async Task<List<Veicolo>> RichiediVeicoli()
    {
        var veicoli = await _context.Veicoli.ToListAsync();
        return veicoli;
    }

    public async Task<List<Veicolo>> RichiediVeicoli(Guid id)
    {
        var veicoli = await _context.Veicoli
            .Where(v => v.IdUtente == id)
            .ToListAsync();

        return veicoli;
    }
}
