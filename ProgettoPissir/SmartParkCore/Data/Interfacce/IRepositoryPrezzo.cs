namespace SmartParkCore.Data.Interfacce;

public interface IRepositoryPrezzo
{
    public Task<decimal> PrezzoSosta();
    public Task<decimal> PrezzoRicarica();
    public Task ModificaPrezzi(Prezzo prezzo);
    public Task<Prezzo> RichiediPrezzi();
}
