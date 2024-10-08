namespace SmartParkCore.Data.Interfacce;

public interface IRepositorySosta
{
    public Task InserisciSosta(Sosta sosta);

    public Task<int> ContaSosteInCorso(Guid id);

    public Task<Sosta> RichiediSostaInCorsoVeicolo(string targa);

    public Task InserisciSostaCompletata(Sosta sosta);

    public Task<List<Sosta>> RichiediSosteUtente(Guid id);

    public Task<List<Sosta>> RichiediSosteInCorso(Guid id);

    public Task<List<Sosta>> RichiediSosteInCorso();

    public Task<List<Sosta>> RichiediSosteUtente();

}
