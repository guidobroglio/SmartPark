using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartParkCore.Data;
using SmartParkCore.Data.Repository;
using SmartParkCore.Data.Interfacce;
using SmartParkCore.Models.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace SmartParkTest.TestRepository;

[TestClass]
public class SostaTest
{
    private DataContext _context;
    private IRepositorySosta _repositorySosta;

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
        var logger = loggerFactory.CreateLogger<RepositorySosta>();

        _repositorySosta = new RepositorySosta(_context, logger);

        _context.Posti.RemoveRange(_context.Posti);
        _context.Veicoli.RemoveRange(_context.Veicoli);
        _context.Soste.RemoveRange(_context.Soste);
        _context.SaveChanges();
    }

    [TestMethod]
    public async Task TestInserisciSosta()
    {
        var posto = new Posto { Id = 1 };
        var veicolo = new Veicolo { Targa = "ABC123" };

        await _context.Posti.AddAsync(posto);
        await _context.Veicoli.AddAsync(veicolo);
        await _context.SaveChangesAsync();

        var sosta = new Sosta
        {
            Id = Guid.NewGuid(),
            Posto = posto,
            Veicolo = veicolo,
            DataInizio = DateTime.Now,
        };

        await _repositorySosta.InserisciSosta(sosta);
        var result = await _context.Soste.FindAsync(sosta.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(sosta.Posto.Id, result.Posto.Id);
        Assert.AreEqual(sosta.Veicolo.Targa, result.Veicolo.Targa);
    }

    [TestMethod]
    public async Task TestSosteInCorso()
    {
        var idUtente = Guid.Parse("a026feec-bd20-4d5a-b4ce-9a66948e0fd0");
        var posto = new Posto { Id = 1 };
        var veicolo = new Veicolo { Targa = "ABC123" };

        await _context.Posti.AddAsync(posto);
        await _context.Veicoli.AddAsync(veicolo);
        await _context.SaveChangesAsync();

        var sosta = new Sosta
        {
            Id = Guid.NewGuid(),
            Posto = posto,
            Veicolo = veicolo,
            IdUtente = idUtente,
            DataInizio = DateTime.Now,
            PrezzoSosta = 0
        };

        await _context.Soste.AddAsync(sosta);
        await _context.SaveChangesAsync();

        var inserimentoSosta = await _context.Soste.FindAsync(sosta.Id);
        Console.WriteLine($"Sosta inserita: {inserimentoSosta?.Id}");

        var soste = await _repositorySosta.RichiediSosteInCorso(idUtente);

        Console.WriteLine($"Numero di soste in corso: {soste}");

        Assert.AreEqual(sosta.Posto.Id, soste.Posto.Id);
    }

    [TestMethod]
    public async Task TestSosteInCorsoVeicolo()
    {
        var idUtente = Guid.Parse("a026feec-bd20-4d5a-b4ce-9a66948e0fd0");
        var posto = new Posto { Id = 1 };
        var veicolo = new Veicolo { Targa = "ABC123" };

        await _context.Posti.AddAsync(posto);
        await _context.Veicoli.AddAsync(veicolo);
        await _context.SaveChangesAsync();

        var sosta = new Sosta
        {
            Id = Guid.NewGuid(),
            Posto = posto,
            Veicolo = veicolo,
            IdUtente = idUtente,
            DataInizio = DateTime.Now,
            PrezzoSosta = 0
        };

        await _context.Soste.AddAsync(sosta);
        await _context.SaveChangesAsync();

        var inserimentoSosta = await _context.Soste.FindAsync(sosta.Id);
        Console.WriteLine($"Sosta inserita: {inserimentoSosta?.Id}");

        var sostaInCorso = await _repositorySosta.RichiediSostaInCorsoVeicolo("ABC123");
        Console.WriteLine("Sosta in corso trovata: " + sostaInCorso?.Id);

        Assert.IsNotNull(sostaInCorso);
        Assert.AreEqual(sosta.Id, sostaInCorso.Id);
    }

    [TestMethod]
    public async Task TestInserisciSostaCompletata()
    {
        var idUtente = Guid.Parse("a026feec-bd20-4d5a-b4ce-9a66948e0fd0");
        var posto = new Posto { Id = 1 };
        var veicolo = new Veicolo { Targa = "ABC123" };

        await _context.Posti.AddAsync(posto);
        await _context.Veicoli.AddAsync(veicolo);
        await _context.SaveChangesAsync();

        var sosta = new Sosta
        {
            Id = Guid.NewGuid(),
            Posto = posto,
            Veicolo = veicolo,
            IdUtente = idUtente,
            DataInizio = DateTime.Now.AddHours(-2), // Sosta iniziata 2 ore fa
            PrezzoSosta = 0 // Indica che la sosta è in corso
        };

        await _context.Soste.AddAsync(sosta);
        await _context.SaveChangesAsync();

        var inserimentoSosta = await _context.Soste.FindAsync(sosta.Id);
        Console.WriteLine($"Sosta inserita: {inserimentoSosta?.Id}");

        // Calcola la durata e il prezzo della sosta
        var dataFine = DateTime.Now;
        var durata = (dataFine - sosta.DataInizio).TotalHours;
        var prezzoSosta = (int)durata * 5; // Supponendo un costo di 5 unità di valuta per ora

        // Aggiorna la sosta con i nuovi valori
        sosta.DataFine = dataFine;
        sosta.Durata = (int)durata;
        sosta.PrezzoSosta = prezzoSosta;

        await _repositorySosta.InserisciSostaCompletata(sosta);

        var sostaCompletata = await _context.Soste.FindAsync(sosta.Id);
        Console.WriteLine($"Sosta completata: {sostaCompletata?.Id}, Prezzo: {sostaCompletata?.PrezzoSosta}");

        Assert.IsNotNull(sostaCompletata);
        Assert.AreEqual(sosta.Id, sostaCompletata.Id);
        Assert.IsNotNull(sostaCompletata.DataFine); // Verifica che DataFine non sia null
        Assert.AreEqual(10, sostaCompletata.PrezzoSosta); // 2 ore * 5 unità di valuta per ora
    }

    [TestMethod]
    public async Task TestRichiediSosteUtente()
    {
        var idUtente = Guid.Parse("a026feec-bd20-4d5a-b4ce-9a66948e0fd0");
        var posto = new Posto { Id = 1 };
        var veicolo = new Veicolo { Targa = "ABC123" };
        var sosta = new Sosta {
            Id = Guid.NewGuid(),
            Posto = posto,
            Veicolo = veicolo,
            IdUtente = idUtente,
            DataInizio = DateTime.Now,
            PrezzoSosta = 0
        };

        await _context.Posti.AddAsync(posto);
        await _context.Veicoli.AddAsync(veicolo);
        await _context.Soste.AddAsync(sosta);
        await _context.SaveChangesAsync();

        var inserimentoSosta = await _context.Soste.FindAsync(sosta.Id);
        Console.WriteLine($"Sosta inserita: {inserimentoSosta?.Id}");
        var sosteUtente = await _repositorySosta.RichiediSosteUtente(idUtente);
        Console.WriteLine($"Soste utente {idUtente}: {sosteUtente.Count}");

        Assert.AreEqual(1, sosteUtente.Count);

        var sostaUtente = sosteUtente[0];
        Assert.AreEqual(sosta.Id, sostaUtente.Id);

        // Aggiungi una nuova sosta per lo stesso utente
        var sosta2 = new Sosta
        {
            Id = Guid.NewGuid(),
            Posto = posto,
            Veicolo = veicolo,
            IdUtente = idUtente,
            DataInizio = DateTime.Now,
            PrezzoSosta = 0
        };

        await _context.Soste.AddAsync(sosta2);
        await _context.SaveChangesAsync();

        var inserimentoSosta2 = await _context.Soste.FindAsync(sosta2.Id);
        Console.WriteLine($"Sosta inserita: {inserimentoSosta2?.Id}");
        var sosteUtente2 = await _repositorySosta.RichiediSosteUtente(idUtente);
        Console.WriteLine($"Soste utente {idUtente}: {sosteUtente2.Count}");

        Assert.AreEqual(2, sosteUtente2.Count);

        var sostaUtente2 = sosteUtente2[1];
        Assert.AreEqual(sosta2.Id, sostaUtente2.Id);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Posti.RemoveRange(_context.Posti);
        _context.Veicoli.RemoveRange(_context.Veicoli);
        _context.Soste.RemoveRange(_context.Soste);
        _context.SaveChanges();
    }
}
