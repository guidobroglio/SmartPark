namespace SmartParkCore.Data.Interfacce;

public interface IRepositoryPrenotazione
{
    public Task<List<Prenotazione>> RichiediPrenotazioniValide(Guid idUtente, DateTime data);
    public Task<List<Prenotazione>> RichiediPrenotazioniInSospeso(Guid idUtente);
    public Task<List<Prenotazione>> RichiediPrenotazioniInSospeso();
    public Task<int> RichiediPostiPrenotati(DateTime data);
    public Task InserisciPrenotazione(Prenotazione prenotazione);
    public Task CompletaPrenotazione(Prenotazione prenotazione);
}
