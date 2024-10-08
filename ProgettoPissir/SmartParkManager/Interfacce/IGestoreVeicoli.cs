namespace SmartParkManager.Interfacce;

public interface IGestoreVeicoli
{
    public Task InserisciVeicoli(List<Veicolo> veicoli);
    public Task<List<Veicolo>> RichiediVeicoli(Guid id);
    public Task ModificaVeicoli(List<Veicolo> veicoli);
}
