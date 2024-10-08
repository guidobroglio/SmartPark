using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartParkCore.Data;
using SmartParkCore.Data.Repository;
using SmartParkCore.Data.Interfacce;
using SmartParkCore.Models.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace SmartParkTest.TestRepository;

[TestClass]
public class RicaricaTest
{
    private DataContext _context;
    private IRepositoryRicarica _repositoryRicarica;
    private ILogger<RepositoryRicarica> _logger;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite("Data Source=../../../../SmartParkDBTest.sqlite")
            .Options;

        _context = new DataContext(options);

        try
        {
            _context.Database.OpenConnection();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Errore connessione al Database: {ex.Message}");
        }

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        var logger = loggerFactory.CreateLogger<RepositoryRicarica>();

        _repositoryRicarica = new RepositoryRicarica(_context, logger);
    }

    [TestMethod]
    public async Task TestRichiediRicaricheUtente()
    {
        var id = Guid.NewGuid();
        var ricarica = new Ricarica
        {
            Id = Guid.NewGuid(),
            IdUtente = id,
            IdPosto = 1,
            PrezzoRicarica = 10,
            PercentualeRicarica = 50,
            Veicolo = new Veicolo { Targa = "ABC123" },
            DataInizio = DateTime.Now,
            DataFine = DateTime.Now,
            Completata = true
        };

        await _context.Ricariche.AddAsync(ricarica);
        await _context.SaveChangesAsync();

        var ricariche = await _repositoryRicarica.RichiediRicaricheUtente(id);
        Assert.AreEqual(1, ricariche.Count);
    }

    [TestMethod]
    public async Task TestRichiediRicaricaInCorso()
    {
        var id = Guid.NewGuid();
        var ricarica = new Ricarica
        {
            Id = Guid.NewGuid(),
            IdUtente = id,
            IdPosto = 1,
            PrezzoRicarica = 10,
            PercentualeRicarica = 50,
            Veicolo = new Veicolo { Targa = "ABC123" },
            DataInizio = DateTime.Now,
            DataFine = DateTime.Now,
            Completata = false
        };

        await _context.Ricariche.AddAsync(ricarica);
        await _context.SaveChangesAsync();

        var ricaricaInCorso = await _repositoryRicarica.RichiediRicaricaInCorso(id);
        Assert.IsNotNull(ricaricaInCorso);
    }

    [TestMethod]
    public async Task TestRichiediRicaricheDaPagare()
    {
        var id = Guid.NewGuid();
        var dataInizio = DateTime.Now.AddDays(-1);
        var dataFine = DateTime.Now.AddDays(1);
        var ricarica = new Ricarica
        {
            Id = Guid.NewGuid(),
            IdUtente = id,
            IdPosto = 1,
            PrezzoRicarica = 10,
            PercentualeRicarica = 50,
            Veicolo = new Veicolo { Targa = "ABC123" },
            DataInizio = DateTime.Now,
            DataFine = DateTime.Now.AddHours(1),
            Completata = true
        };

        await _context.Ricariche.AddAsync(ricarica);
        await _context.SaveChangesAsync();

        var repository = new RepositoryRicarica(_context, _logger);

        var ricariche = await repository.RichiediRicaricheDaPagare(id, dataInizio, dataFine);

        Assert.AreEqual(1, ricariche.Count);
        Assert.AreEqual(ricarica.Id, ricariche.First().Id);
    }

    [TestMethod]
    public async Task TestInserisciRicaricaCompletata()
    {
        var veicolo = new Veicolo { Targa = "ABC123", PercentualeBatteria = 50 };
        await _context.Veicoli.AddAsync(veicolo);
        await _context.SaveChangesAsync();

        var ricarica = new Ricarica
        {
            Id = Guid.NewGuid(),
            IdUtente = Guid.NewGuid(),
            IdPosto = 1,
            PrezzoRicarica = 10,
            PercentualeRicarica = 50,
            Veicolo = veicolo,
            DataInizio = DateTime.Now,
            DataFine = DateTime.Now,
            Completata = false
        };

        await _context.Ricariche.AddAsync(ricarica);
        await _context.SaveChangesAsync();

        ricarica.Completata = true;
        ricarica.Veicolo.PercentualeBatteria = 100;

        await _repositoryRicarica.InserisciRicaricaCompletata(ricarica);
        var result = await _context.Ricariche.FindAsync(ricarica.Id);

        Assert.IsTrue(result.Completata);
        Assert.AreEqual(100, result.Veicolo.PercentualeBatteria);
    }

    [TestMethod]
    public async Task TestProssimaRicarica()
    {
        var veicolo = new Veicolo { Targa = "ABC123", PercentualeBatteria = 50 };
        await _context.Veicoli.AddAsync(veicolo);
        await _context.SaveChangesAsync();

        var ricarica = new Ricarica
        {
            Id = Guid.NewGuid(),
            IdUtente = Guid.NewGuid(),
            IdPosto = 1,
            PrezzoRicarica = 10,
            PercentualeRicarica = 50,
            Veicolo = veicolo,
            DataInizio = DateTime.Now,
            DataFine = DateTime.Now,
            Completata = false
        };

        await _context.Ricariche.AddAsync(ricarica);
        await _context.SaveChangesAsync();

        var prossimaRicarica = await _repositoryRicarica.ProssimaRicarica();
        Assert.IsNotNull(prossimaRicarica);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Ricariche.RemoveRange(_context.Ricariche);
        _context.Veicoli.RemoveRange(_context.Veicoli);
        _context.SaveChanges();
    }
}