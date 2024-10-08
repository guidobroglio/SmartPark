namespace SmartParkCore.Data.Interfacce;

public interface IRepositoryVeicolo
{
    public Task<string> RichiediTargaUtente(Guid id);
    public Task<Guid> RichiediUtenteTarga(string targa);
    public Task InserisciVeicolo(Veicolo veicolo);
    public Task ModificaVeicolo(Veicolo veicolo);
    public Task<List<Veicolo>> RichiediVeicoli();
    public Task<List<Veicolo>> RichiediVeicoli(Guid id);
}
