namespace SmartParkCore.Data.Interfacce;

public interface IRepositoryPagamento
{
    public Task InserisciPagamento(Pagamento pagamento);
    public Task<Pagamento> RichiediPagamentiInSospeso(Guid idUtenteRichiesta);
    public Task<List<Pagamento>> RichiediPagamenti();
    public Task<List<Pagamento>> RichiediPagamentiUtente(Guid id);
}
