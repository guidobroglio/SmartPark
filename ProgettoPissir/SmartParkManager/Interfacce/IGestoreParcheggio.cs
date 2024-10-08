using SmartParkCore.Models.Json;

namespace SmartParkManager.Interfacce;

public interface IGestoreParcheggio
{
    public Task<int> InserisciSosta(Sosta sosta);
    public Task<int> CalcolaPosto();
    public Task<IList<OccupazioneJson>> RichiediOccupazionePosti();
    public Task<int> InserisciSostaPrenotata(Sosta sosta);
    public Task<List<Sosta>> RichiediSosteUtente(Guid id);
    public Task<List<Sosta>> RichiediSoste();
    public Task<List<Sosta>> RichiediSosteInCorso(Guid id);
    public Task<List<Sosta>> RichiediSosteInCorso();

}
