using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartParkCore.Data;
using SmartParkCore.Data.Repository;
using SmartParkCore.Data.Interfacce;
using SmartParkCore.Models.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace SmartParkTest.TestRepository;

[TestClass]
public class PostoTest
{
    private DataContext _context;
    private IRepositoryPosto _repositoryPosto;

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
        var logger = loggerFactory.CreateLogger<RepositoryPosto>();

        _repositoryPosto = new RepositoryPosto(_context, logger);

        _context.Posti.RemoveRange();
        _context.SaveChanges();
    }

    /*
    [TestMethod]
    public async Task TestImpostaOccupato()
    {
        var posto = new Posto { Id = 4 , Disponibile = true};

        await _context.Posti.AddAsync(posto);
        await _context.SaveChangesAsync();

        await _repositoryPosto.ImpostaOccupato(posto);

        var postoOccupato = await _context.Posti.Where(p => p.Id == 3).FirstAsync();

        Assert.IsFalse(postoOccupato.Disponibile);
    }*/

    [TestMethod]
    public async Task TestImpostaDisponibile()
    {
        _context.Posti.RemoveRange(_context.Posti);
        _context.SaveChanges();
        await _context.SaveChangesAsync();

        var posto = new Posto { Id = 1 };

        await _context.Posti.AddAsync(posto);
        await _context.SaveChangesAsync();

        await _repositoryPosto.ImpostaDisponibile(posto);

        var postoDisponibile = await _context.Posti.Where(p => p.Id == 1).FirstAsync();

        Assert.IsTrue(postoDisponibile.Disponibile);
    }

    [TestMethod]
    public async Task TestPostiOccupati()
    {
        _context.Posti.RemoveRange(_context.Posti);
        _context.SaveChanges();
        await _context.SaveChangesAsync();

        var posto = new Posto { Id = 1 };

        await _context.Posti.AddAsync(posto);
        await _context.SaveChangesAsync();

        var numeroPostiOccupati = await _repositoryPosto.PostiOccupati();

        Assert.AreEqual(0, numeroPostiOccupati);
    }

    [TestMethod]
    public async Task TestPostiOccupatiConPostoOccupato()
    {
        _context.Posti.RemoveRange(_context.Posti);
        _context.SaveChanges();
        await _context.SaveChangesAsync();

        var posto = new Posto { Id = 3 };

        await _context.Posti.AddAsync(posto);
        await _context.SaveChangesAsync();

        await _repositoryPosto.ImpostaOccupato(posto);

        var numeroPostiOccupati = await _repositoryPosto.PostiOccupati();

        Assert.AreEqual(1, numeroPostiOccupati);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Posti.RemoveRange();
        _context.SaveChanges();
    }
}
