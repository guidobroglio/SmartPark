using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartParkCore.Data;
using SmartParkCore.Data.Repository;
using SmartParkCore.Data.Interfacce;
using SmartParkCore.Models.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace SmartParkTest.TestRepository;

[TestClass]
public class PagamentoTest
{
    private DataContext _context;
    private IRepositoryPagamento _repositoryPagamento;

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
        var logger = loggerFactory.CreateLogger<RepositoryPagamento>();

        _repositoryPagamento = new RepositoryPagamento(_context, logger);
    }

    [TestMethod]
    public async Task TestInserisciPagamento()
    {
        var pagamento = new Pagamento
        {
            Id = Guid.NewGuid(),
            IdUtente = Guid.NewGuid(),
            ImportoSosta = 5,
            ImportoRicarica = 0,
            Data = DateTime.Now,
            Veicolo = new Veicolo { Targa = "ABC123" }
        };

        await _repositoryPagamento.InserisciPagamento(pagamento);

        var pagamentoInserito = await _context.Pagamenti.FindAsync(pagamento.Id);

        Assert.IsNotNull(pagamentoInserito);
        Assert.AreEqual(pagamento.Id, pagamentoInserito.Id);
        Assert.AreEqual(pagamento.IdUtente, pagamentoInserito.IdUtente);
        Assert.AreEqual(pagamento.ImportoSosta, pagamentoInserito.ImportoSosta);
        Assert.AreEqual(pagamento.ImportoRicarica, pagamentoInserito.ImportoRicarica);
        Assert.AreEqual(pagamento.Data, pagamentoInserito.Data);
        Assert.AreEqual(pagamento.Veicolo.Targa, pagamentoInserito.Veicolo.Targa);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Pagamenti.RemoveRange(_context.Pagamenti);
        _context.SaveChanges();
    }
}

