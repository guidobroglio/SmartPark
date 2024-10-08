using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartParkCore.Data;
using SmartParkCore.Data.Repository;
using SmartParkCore.Data.Interfacce;
using SmartParkCore.Models.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace SmartParkTest.TestRepository;

[TestClass]
public class VeicoloTest
{
    private DataContext _context;
    private IRepositoryVeicolo _repositoryVeicolo;
    private ILogger<RepositoryVeicolo> _logger;

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

        _logger = loggerFactory.CreateLogger<RepositoryVeicolo>();

        _repositoryVeicolo = new RepositoryVeicolo(_context, _logger);

        _context.Veicoli.RemoveRange(_context.Veicoli);
        _context.SaveChanges();
    }

    [TestMethod]
    public async Task TestInserisciVeicolo()
    {


        var veicolo = new Veicolo { Targa = "ABC123" };

        await _repositoryVeicolo.InserisciVeicolo(veicolo);

        var veicoli = await _context.Veicoli.ToListAsync();

        Assert.AreEqual(1, veicoli.Count);

        _context.Veicoli.RemoveRange(_context.Veicoli);
        _context.SaveChanges();
    }

    [TestMethod]
    public async Task TestRichiediVeicoli()
    {
        var veicolo = new Veicolo { Targa = "ABC123" };

        await _repositoryVeicolo.InserisciVeicolo(veicolo);

        var veicoli = await _repositoryVeicolo.RichiediVeicoli();

        Assert.AreEqual(1, veicoli.Count);

        _context.Veicoli.RemoveRange(_context.Veicoli);
        _context.SaveChanges();
    }

    [TestMethod]
    public async Task TestRichiediTargaUtente()
    {
        var veicolo = new Veicolo { Targa = "ABC123" };

        await _repositoryVeicolo.InserisciVeicolo(veicolo);

        var targa = await _repositoryVeicolo.RichiediTargaUtente(veicolo.IdUtente);

        Assert.AreEqual("ABC123", targa);

        _context.Veicoli.RemoveRange(_context.Veicoli);
        _context.SaveChanges();
    }

    [TestMethod]
    public async Task TestRichiediUtenteTarga()
    {
        var userId = Guid.NewGuid();
        var utente = new Utente { Id = userId, Nome = "Mario Rossi" };
        var veicolo = new Veicolo { Targa = "ABC123", IdUtente = userId };

        await _context.Utenti.AddAsync(utente);
        await _context.Veicoli.AddAsync(veicolo);
        await _context.SaveChangesAsync();

        var utenteInserito = await _context.Utenti.FindAsync(userId);
        var veicoloInserito = await _context.Veicoli.FirstOrDefaultAsync(v => v.Targa == "ABC123");

        Assert.IsNotNull(utenteInserito, "L'utente non è stato inserito correttamente nel contesto del database.");
        Assert.IsNotNull(veicoloInserito, "Il veicolo non è stato inserito correttamente nel contesto del database.");
        Assert.AreEqual(userId, veicoloInserito.IdUtente, "Il campo IdUtente del veicolo non è stato impostato correttamente.");

        Console.WriteLine($"Utente inserito: {utenteInserito.Nome}, ID: {utenteInserito.Id}");
        Console.WriteLine($"Veicolo inserito: {veicoloInserito.Targa}, ID Utente: {veicoloInserito.IdUtente}");

        var id = await _repositoryVeicolo.RichiediUtenteTarga(veicolo.Targa);

        Assert.AreEqual(userId, id);

        _context.Veicoli.RemoveRange(_context.Veicoli);
        _context.SaveChanges();
    }
}    