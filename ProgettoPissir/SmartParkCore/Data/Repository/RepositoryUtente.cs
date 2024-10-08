using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartParkCore.Security;

namespace SmartParkCore.Data.Repository;

public class RepositoryUtente : IRepositoryUtente
{
    readonly DataContext _context;
    readonly ILogger<RepositoryUtente> _logger;

    public RepositoryUtente(DataContext context, ILogger<RepositoryUtente> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InserisciUtente(Utente utente)
    {
        await _context.Utenti.AddAsync(utente);
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Repository utente: inserita nuovo utente");
        }
        catch (Exception e)
        {
            _logger.LogError($"Repository utente: {e.Message}");
        }
    }

    public async Task ImpostaUtentePremium(Guid id)
    {
        await _context.Utenti
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(modifiche => modifiche
            .SetProperty(u=>u.Id,id));

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Repository utente: impostato utente {id} con livello Premium.");
        }
        catch (Exception e)
        {
            _logger.LogError($"Repository utente: {e.Message}");
        }
    }

    public async Task<Utente> RichiediUtente(string email, string password)
    {
        var utente = await _context.Utenti
            .Where(u => u.Email == email).FirstOrDefaultAsync();

        return utente;
    }
    
    public async Task<Utente> RichiediUtente(string email)
    {
        var utente = await _context.Utenti
            .Where(u => u.Email == email).FirstOrDefaultAsync();

        return utente;
    }
}
