namespace SmartParkManager.Interfacce;

public interface IGestoreMonitor
{
    public Task InviaOccupazioneParcheggio(int posto, string disponibile);

}
