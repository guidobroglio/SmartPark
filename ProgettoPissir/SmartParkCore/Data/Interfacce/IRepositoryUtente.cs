namespace SmartParkCore.Data.Interfacce;

public interface IRepositoryUtente
{
    public Task InserisciUtente(Utente utente);
    public Task<Utente> RichiediUtente(string email, string password);
    public Task<Utente> RichiediUtente(string email);
    public Task ImpostaUtentePremium(Guid id);
}
