using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartParkCore.Data;
using SmartParkCore.Data.Repository;
using SmartParkCore.Data.Interfacce;
using SmartParkCore.Models.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace SmartParkTest.TestRepository;

[TestClass]
public class PrenotazioneTest
{
    private DataContext _context;
    private IRepositoryPrenotazione _repositoryPrenotazione;

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
        var logger = loggerFactory.CreateLogger<RepositoryPrenotazione>();

        _repositoryPrenotazione = new RepositoryPrenotazione(_context,logger);
    }

    [TestMethod]
    public async Task TestInserisciPrenotazione()
    {
        var prenotazione = new Prenotazione
        {
            Id = Guid.NewGuid(),
            IdUtente = Guid.NewGuid(),
            DataInizio = DateTime.Now,
            DataFine = DateTime.Now.AddHours(1),
            Stato = true
        };

        await _repositoryPrenotazione.InserisciPrenotazione(prenotazione);

        var prenotazioni = await _context.Prenotazioni.ToListAsync();

        Assert.AreEqual(1, prenotazioni.Count);
    }

    [TestMethod]
    public async Task TestPostiPrenotati()
    {
        var prenotazione = new Prenotazione
        {
            Id = Guid.NewGuid(),
            IdUtente = Guid.NewGuid(),
            DataInizio = DateTime.Now,
            DataFine = DateTime.Now.AddHours(1),
            Stato = true
        };

        await _repositoryPrenotazione.InserisciPrenotazione(prenotazione);

        var postiPrenotati = await _repositoryPrenotazione.RichiediPostiPrenotati(DateTime.Now);

        Assert.AreEqual(1, postiPrenotati);
    }

    [TestMethod]
    public async Task TestRichiediPrenotazioniValide()
    {
        var prenotazione = new Prenotazione
        {
            Id = Guid.NewGuid(),
            IdUtente = Guid.NewGuid(),
            DataInizio = DateTime.Now,
            DataFine = DateTime.Now.AddHours(1),
            Stato = false
        };

        await _repositoryPrenotazione.InserisciPrenotazione(prenotazione);

        var prenotazioni = await _repositoryPrenotazione.RichiediPrenotazioniValide(prenotazione.IdUtente, DateTime.Now);

        Assert.AreEqual(1, prenotazioni.Count);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Posti.RemoveRange(_context.Posti);
        _context.Veicoli.RemoveRange(_context.Veicoli);
        _context.Prenotazioni.RemoveRange(_context.Prenotazioni);
        _context.SaveChanges();
    }
}
