namespace SmartParkManager.Interfacce;

public interface IGestoreRicariche
{
    public Task<int> RicaricheInCoda();
    public Task<Ricarica> RicaricaInCorso(Guid id);
    public Task<List<Ricarica>> RicaricheUtente(Guid id);
    public Task<List<Ricarica>> RicaricheUtenti();
    public Task<Ricarica> ProssimaRicarica();
}
