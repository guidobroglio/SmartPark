using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SmartParkCore.Data.Repository;

public class RepositoryPagamento : IRepositoryPagamento
{
    readonly DataContext _context;
    readonly ILogger<RepositoryPagamento> _logger;

    public RepositoryPagamento(DataContext context, ILogger<RepositoryPagamento> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InserisciPagamento(Pagamento pagamento)
    {
        _logger.LogInformation($"Repository pagamento: inserimento pagamento per l'utente {pagamento.IdUtente}");
        await _context.Pagamenti.AddAsync(pagamento);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Pagamento>> RichiediPagamenti()
    {
        return await _context.Pagamenti
            .Include(p => p.Veicolo)
            .ToListAsync();
    }

    public async Task<List<Pagamento>> RichiediPagamentiUtente(Guid id)
    {
        return await _context.Pagamenti
            .Include(p => p.Veicolo)
            .Where(p => p.IdUtente == id)
            .ToListAsync();
    }

    public Task<Pagamento> RichiediPagamentiInSospeso(Guid idUtenteRichiesta)
    {
        // TODO forse non serve
        throw new NotImplementedException();
    }
}

