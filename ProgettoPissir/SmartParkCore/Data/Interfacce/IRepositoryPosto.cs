using SmartParkCore.Models.Entities;
using SmartParkCore.Models.Json;

namespace SmartParkCore.Data.Interfacce;

public interface IRepositoryPosto
{
    public Task ImpostaOccupato(Posto posto);
    public Task ImpostaDisponibile(Posto posto);
    public Task<IList<Posto>> PostiOccupati();
    public Task<IList<OccupazioneJson>> RichiediOccupazionePosti();
}
