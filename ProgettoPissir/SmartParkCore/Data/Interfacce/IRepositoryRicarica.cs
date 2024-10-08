namespace SmartParkCore.Data.Interfacce;

public interface IRepositoryRicarica
{
    public Task<List<Ricarica>> RichiediRicaricheUtente(Guid id);
    public Task<List<Ricarica>> RichiediRicariche();
    public Task<Ricarica> RichiediRicaricaInCorso(Guid id);
    public Task InserisciRicaricaCompletata(Ricarica ricarica);
    public Task<Ricarica> ProssimaRicarica();
    public Task<int> RichiediRicaricheInCoda();
    public Task InserisciRichiestaRicarica(Ricarica ricarica);
    public Task<List<Ricarica>> RichiediRicaricheDaPagare(Guid id, DateTime dataInizio, DateTime dataFine);

}
