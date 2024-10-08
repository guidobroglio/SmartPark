namespace SmartParkManager.Interfacce;

public interface IGestorePrenotazioni
{
    Task<int> InserisciPrenotazione(Prenotazione prenotazione);
    public Task<List<Prenotazione>> RichiediPrenotazioniInSospeso(Guid id);
    public Task<List<Prenotazione>> RichiediPrenotazioniInSospeso();
}
