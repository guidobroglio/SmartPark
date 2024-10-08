namespace SmartParkManager.Interfacce;

public interface IGestoreMWbot
{
    public Task AvviaGestoreRicariche();
    public Task<int> RichiediRicarica(Ricarica ricarica);
}
