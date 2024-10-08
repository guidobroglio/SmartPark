namespace SmartParkManager.Interfacce;

public interface IGestorePrezzi
{
    public Task<Prezzo> RichiediPrezzi();
    public Task ModificaPrezzi(Prezzo prezzo);
}
