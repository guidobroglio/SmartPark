namespace SmartParkManager.Interfacce;

public interface IGestorePagamenti
{
    public Task<int> InserisciPagamento(string targa);
    public Task<List<Pagamento>> RichiediPagamenti();
    public Task<List<Pagamento>> RichiediPagamentiUtente(Guid id);
}
