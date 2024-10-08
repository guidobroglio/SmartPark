using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SmartParkCore.Data;

public class DataContext : IdentityDbContext<Utente, IdentityRole<Guid>, Guid>
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    { }

    public DbSet<Utente> Utenti => Set<Utente>();
    public DbSet<Pagamento> Pagamenti => Set<Pagamento>();
    public DbSet<Prenotazione> Prenotazioni => Set<Prenotazione>();
    public DbSet<Ricarica> Ricariche => Set<Ricarica>();
    public DbSet<Sosta> Soste => Set<Sosta>();
    public DbSet<Posto> Posti => Set<Posto>();
    public DbSet<Veicolo> Veicoli => Set<Veicolo>();
    public DbSet<Prezzo> Prezzi => Set<Prezzo>();
}
